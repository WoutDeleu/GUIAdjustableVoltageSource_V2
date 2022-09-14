using AdjustableVoltageSource;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;

public class Communicator
{
    private int baudrate = 115200;
    public SerialPort serialPort;
    public MainWindow mw;
    public bool connectionSuccesfull;
    public enum Functions
    {
        PUT_VOLTAGE = 1,
        CONNECT_TO_GROUND = 2,
        CONNECT_TO_BUS = 3,
        MEASURE_VOLTAGE = 4,
        MEASURE_CURRENT = 5,
        CHANGE_BOARDNUMBER = 6,
        GET_BOARDNUMBER = 7,
        DISCONNECT_VOLTAGE = 8,
        RESET = 9
    }
    public Communicator()
    {
        serialPort = new SerialPort();
        serialPort.BaudRate = baudrate;
        mw = (MainWindow)Application.Current.MainWindow;
    }
    
    public void closeSerialPort()
    {
        if (!serialPort.IsOpen) serialPort.Open();
        serialPort.DiscardOutBuffer();
        serialPort.DiscardInBuffer();

        Thread.Sleep(500);

        serialPort.Dispose();
        serialPort.Close();
    }
    public void writeSerialPort(string data)
    {
        try
        {
            if (serialPort.IsOpen)
            {
                Thread.Sleep(50);
                serialPort.WriteLine(data);
                switch(data[0])
                {
                    case '1':
                        mw.CommandBox = "Put Voltage: \t\t" + data;
                        break;
                    case '2':
                        mw.CommandBox = "Connect to Gnd: \t" + data;
                        break;
                    case '3':
                        mw.CommandBox = "Connect to Bus: \t" + data;
                        break;
                    case '4':
                        mw.CommandBox = "Measure Voltage: \t" + data;
                        break;
                    case '5':
                        mw.CommandBox = "Measure Current: \t" + data;
                        break;
                    case '6':
                        mw.CommandBox = "Change BoardNumber: \t" + data;
                        break;
                    case '7':
                        mw.CommandBox = "Get BoardNumber: \t" + data;
                        break;
                    case '8':
                        mw.CommandBox = "Disconnect Voltage: \t" + data;
                        break;
                    case '9':
                        mw.CommandBox = "RESET: \t" + data;
                        break;
                    default:
                        break;
                }
                Thread.Sleep(50);
            }
            else
            {
                mw.StatusBox_Error = "FAULT in connection. SerialPort is closed.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    public void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string readSciMessage = "";
        try
        {
            if (serialPort.IsOpen)
            {
                Thread.Sleep(200);
                do
                {
                    readSciMessage += serialPort.ReadExisting();
                } while (serialPort.BytesToRead != 0);

                Debug.WriteLine(readSciMessage);
                string[] strings = readSciMessage.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                foreach (String str in strings)
                {
                    if (str.Contains("||"))
                    {
                        mw.StatusBox_Error = ExtractErrorMessage(str, mw);
                    }
                    if (str.Contains("##"))
                    {
                        mw.StatusBox_Status = ExtractStatus(str, mw);
                    }
                    if (str.Contains("(("))
                    {
                        mw.RegisterBox = ExtractRegisters(str, mw);
                    }
                }
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.WriteLine(readSciMessage + "\n");
            Debug.WriteLine(ex.ToString() + "\n");
        }
        catch (FormatException ex)
        {
            Debug.WriteLine(readSciMessage + "\n");
            Debug.WriteLine(ex.ToString() + "\n");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(readSciMessage + "\n");
            Debug.WriteLine(ex.ToString() + "\n");
        }
    }


    public void initSerialPort()
    {
        try
        {
            // If serialport is open, it needs to be
            // 
            if (serialPort.IsOpen)
            {
                serialPort.Close();

                if (!serialPort.IsOpen)
                {
                    mw.StatusBox_Status = "'" + serialPort.PortName + "' Port Closed\n";
                }
            }
            // Based on connection method
            if (mw.ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Auto-detect")
            {
                mw.StatusBox_Status = "Auto Detecting Arduino";
                // Autodetect based on Windows Query
                if (Regex.IsMatch(AutodetectArduinoPort(), @"\bCOM\d+\b"))
                {
                    serialPort.PortName = AutodetectArduinoPort();
                    Debug.WriteLine("Port found in Win32 Queruy: " + AutodetectArduinoPort());
                    mw.StatusBox_Status = ("Port found in Win32 Query: " + AutodetectArduinoPort());
                }
                // Autodetect based on Windows Registry
                else 
                { 
                    string comportname = ComPortName("2341", "0042"); 
                    if (Regex.IsMatch(comportname, @"\bCOM\d+\b"))
                    {
                        serialPort.PortName = comportname;
                        mw.StatusBox_Status = ("Port found in Win32 Registery Query : " + serialPort.PortName);
                        Debug.WriteLine("Port found in Win32 Registery Query : " + serialPort.PortName);
                    }
                    else
                    {
                        // FOUTMELDING! GEEN PORTNAME GEVONDEN
                    }
                }
            }
            // Manually input portname
            else if (mw.ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Others")
            {
                serialPort.PortName = mw.newComPort.Text;
                Debug.WriteLine("Port found based on input User : " + serialPort.PortName);
            }
            // Choose portname based on selection in list
            else
            {
                serialPort.PortName = mw.ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                Debug.WriteLine("Port selected based on selection in Combobox : " + mw.ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
                mw.StatusBox_Status = ("Port selected based on selection in Combobox : " + mw.ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
            }
            // Finish setup port
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.BaudRate = 115200;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.Open();
            mw.StatusBox_Status = ("'" + serialPort.PortName + "' Port Opened Succesfully");
            connectionSuccesfull = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n");
            Debug.WriteLine("Port " + serialPort.PortName + " could not be opened");
            mw.StatusBox_Error = ("Port " + serialPort.PortName + " could not be opened");
            connectionSuccesfull = false;
        }
    }

    // Search registery for Arduino based on VID/PID
    string ComPortName(String VID, String PID)
    {
        // Pattern is the format of the usb registery which contains the arduino
        String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
        Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
        List<string> comports = new List<string>();

        // Usb registeries are located here
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

        // Scan all subregisteries in the location for a match the VID & PID
        foreach (String s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (String s in rk3.GetSubKeyNames())
            {
                // If the subdirectiry matches the VID & PID
                if (_rx.Match(s).Success)
                {
                    // Open the correct VID & PID directory
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    // Scan this directory again
                    // This contains all the data regarding the arduino
                    // if there is more than 1 arduino connected, there will be multiple directories present
                    foreach (String s2 in rk4.GetSubKeyNames())
                    {
                        // Open every registery in this directory (open all arduino data registers) and extract the portname
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                        // Format: e.g. COM4
                        comports.Add((string)rk6.GetValue("PortName"));
                        Debug.WriteLine("Portname found in registery : " + (string)rk6.GetValue("PortName"));
                    }
                }
            }
        }
        
        if (comports.Count >= 1)
        {
            // POPUP maken
            String availableComports = "Available COM ports: ";
            foreach(String s in comports)
            {
                Debug.WriteLine("Selection COMports: " +  s);
                availableComports += (s)+ ", ";
            }
            mw.StatusBox_Status = availableComports.Remove(availableComports.Length - 2);
            return "COM5";
        }
        else if (comports.Count == 1)
        {
            return comports[0].ToString();
        }
        else
        {
            mw.StatusBox_Error = "Fault in autofinding COM ports. No COM found (not in Registery and not in the windowsQuery)";
            mw.StatusBox_Error = "Fault in connection";
            return "COM";
        }
    }
    private string AutodetectArduinoPort()
    {
        // Query on windows system searching the arduino
        ManagementScope connectionScope = new ManagementScope();
        SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

        foreach (ManagementObject item in searcher.Get())
        {
            string desc = item["Description"].ToString();
            string deviceId = item["DeviceID"].ToString();
            if (desc.Contains("Arduino Mega"))
            {
                Debug.WriteLine("Device ID " + deviceId);
                return deviceId;
            }
        }
        return "";
    }


    // Filter out input. Remove extra identifier characters
    public static string ExtractInput(string input, MainWindow mainWindow)
    {
        char[] inputArray = input.ToCharArray();
        int begin = 0, end = 0;
        for (int i = 0; i < inputArray.Length - 1; i++)
        {
            if (inputArray[i] == '[') begin = i;
            if (inputArray[i] == ']') end = i;
        }
        if (end != 0)
        {
            return input.Substring(begin + 1, (end - begin - 1));
        }
        else
        {
            mainWindow.StatusBox_Error = "Fault in communication. Could not extract message from input message.";
            return null;
        }
    }    
    public static string ExtractErrorMessage(string errorMessage, MainWindow mainWindow)
    {
        char[] iputArray = errorMessage.ToCharArray();
        int begin = 0, end = 0;
        bool foundBeginning = false;
        for (int i = 0; i < iputArray.Length - 1; i++)
        {
            if (iputArray[i] == '|' && iputArray[i + 1] == '|' && !foundBeginning)
            {
                begin = i + 1;
                foundBeginning = true;
            }
            else if (iputArray[i] == '|' && iputArray[i + 1] == '|' && foundBeginning)
            {
                end = i;
            }
        }
        if (end != 0)
        {
            return errorMessage.Substring(begin + 1, (end - begin - 1));
        }
        else
        {
            mainWindow.StatusBox_Error = "Fault in communication. Could not extract message from Error message.";
            return null;
        }
    }   
    public static string ExtractStatus(string status, MainWindow mainWindow)
    {
        char[] inputArray = status.ToCharArray();
        int begin = 0, end = 0;
        bool foundBeginning = false;
        for (int i = 0; i < inputArray.Length-1; i++)
        {
            if (inputArray[i] == '#' && inputArray[i + 1] == '#' && !foundBeginning) 
            {
                begin = i + 1;
                foundBeginning=true;
            }
            else if (inputArray[i] == '#' && inputArray[i + 1] == '#' && foundBeginning)
            {
                end = i;
            }
        }
        if (end != 0)
        {
            return status.Substring(begin + 1, (end - begin - 1));
        }
        else
        {
            mainWindow.StatusBox_Error = "Fault in communication. Could not extract message from Status message.";
            return null;
        }
    }   
    public static string ExtractRegisters(string registersMessage, MainWindow mainWindow)
    {
        char[] registerArray = registersMessage.ToCharArray();
        int begin = 0, end = 0;
        bool foundBeginning = false;
        for (int i = 0; i < registerArray.Length-1; i++)
        {
            if (registerArray[i] == '(' && registerArray[i + 1] == '(' && !foundBeginning)
            {
                begin = i + 1;
                foundBeginning = true;
            }
            else if (registerArray[i] == ')' && registerArray[i + 1] == ')' && foundBeginning)
            {
                end = i;
            }
        }
        if (end == 0)
        {
            mainWindow.StatusBox_Error = "Fault in communication. Could not extract message from Register message.";
            return null;
        }
        else
        {
            registersMessage = registersMessage.Substring(begin+1, (end - begin - 1));
            if (registersMessage.Contains("::"))
            {
                string[] splitUpRegisters = registersMessage.Split("_");
                string output = "";
                foreach (String str in splitUpRegisters)
                {
                    output += str + "\r";
                }
                return output;
            }
            else return registersMessage;
        }
    }
}

