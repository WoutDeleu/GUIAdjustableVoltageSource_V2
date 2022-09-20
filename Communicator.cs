using AdjustableVoltageSource;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly int baudrate = 115200;
        public SerialPort serialPort;
        public string ret_port;
        public bool isClosing;
        private bool _isConnectionSuccesfull;
        public bool IsConnectionSuccesfull
        {
            get { return _isConnectionSuccesfull; }
            set
            {
                _isConnectionSuccesfull = value;
                UpdateArduinoStatus(value);
            }
        }
        public enum BoardFunctions
        {
            PING = 0,
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

        public void CloseSerialPort()
        {
            if (serialPort.IsOpen)
            {
                isClosing = true;
                serialPort.DiscardOutBuffer();
                serialPort.DiscardInBuffer();

                Thread.Sleep(50);

                serialPort.Dispose();
                serialPort.Close();

                IsConnectionSuccesfull = false;
                StatusBox_Status = "'" + serialPort.PortName + "' Port Closed Succesfully";
                isClosing = false;
            }
            else
            {

                StatusBox_Status = "'" + serialPort.PortName + "' was already Closed";
            }
        }
        public void WriteSerialPort(string data)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    isClosing = true;
                    Thread.Sleep(50);
                    serialPort.WriteLine(data);
                    switch (data[0])
                    {
                        case '0':
                            if(AppTimer.Elapsed.TotalSeconds.ToString().Length>=10) CommandBox = "Ping: \t\t" + data;
                            else CommandBox = "Ping: \t" + data; ;
                            break;
                        case '1':
                            CommandBox = "Put Voltage: \t\t" + data;
                            break;
                        case '2':
                            CommandBox = "Connect to Gnd: \t" + data;
                            break;
                        case '3':
                            CommandBox = "Connect to Bus: \t" + data;
                            break;
                        case '4':
                            CommandBox = "Measure Voltage: \t" + data;
                            break;
                        case '5':
                            CommandBox = "Measure Current: \t" + data;
                            break;
                        case '6':
                            CommandBox = "Change BoardNumber: " + data;
                            break;
                        case '7':
                            CommandBox = "Get BoardNumber: \t" + data;
                            break;
                        case '8':
                            CommandBox = "Disconnect Voltage: \t" + data;
                            break;
                        case '9':
                            CommandBox = "RESET: \t" + data;
                            break;
                        default:
                            break;
                    }
                    Thread.Sleep(50);
                    isClosing = true;
                }
                else
                {
                    throw new Exception("FAULT in connection. SerialPort has been closed.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                StatusBox_Error = ex.Message;

                CloseSerialPort();
                IsConnectionSuccesfull = false;

                ResetWithClosedPort();
            }
        }
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string readSciMessage = "";
            try
            {
                if (serialPort.IsOpen)
                {
                    if (!isClosing)
                    {
                        Thread.Sleep(200);
                        do
                        {
                            if (serialPort.IsOpen) readSciMessage += serialPort.ReadExisting();
                        } while (serialPort.BytesToRead != 0);

                        Debug.WriteLine(readSciMessage);
                        FilterInput(readSciMessage);
                    }
                }
                else
                {
                    throw new Exception("FAULT in connection. SerialPort has been closed.");
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine(readSciMessage + "\n");
                Debug.WriteLine(ex.ToString() + "\n");
            }
            catch (FormatException ex)
            {
                Debug.WriteLine(ex.Message);
                StatusBox_Error = ex.Message;

                CloseSerialPort();
                IsConnectionSuccesfull = false;

                ResetWithClosedPort();
            }
        }


        public void InitSerialPort()
        {
            try
            {
                // If serialport isn't open, it needs to be
                if (serialPort.IsOpen)
                {
                    CloseSerialPort();

                    if (!serialPort.IsOpen)
                    {
                        StatusBox_Status = "'" + serialPort.PortName + "' Port Closed Succesfully";
                    }
                }
                // Choose portname based on connection method
                if (COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Auto-detect")
                {
                    StatusBox_Status = "Auto Detecting Arduino";
                    // Autodetect based on Windows Query
                    if (Regex.IsMatch(AutodetectArduinoPort(), @"\bCOM\d+\b"))
                    {
                        serialPort.PortName = AutodetectArduinoPort();
                        StatusBox_Status = "Port found in Win32 Query: " + AutodetectArduinoPort();
                    }
                    // Autodetect based on Windows Registry
                    else
                    {
                        string comportname = ComPortName("2341", "0042");
                        if (Regex.IsMatch(comportname, @"\bCOM\d+\b"))
                        {
                            serialPort.PortName = comportname;
                            StatusBox_Status = ("Port found/chosen in Win32 Registery Query : " + serialPort.PortName);
                        }
                        else
                        {
                            StatusBox_Error = "No port found while auto detecting";
                        }
                    }
                }
                // Manually input portname
                else if (COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Others")
                {
                    serialPort.PortName = newCOMPortTextBox.Text;
                    StatusBox_Status = ("Port found based on input User : " + serialPort.PortName);
                }
                // Choose portname based on selection in list
                else
                {
                    serialPort.PortName = COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                    Debug.WriteLine("Port selected based on selection in Combobox : " + COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
                    StatusBox_Status = ("Port selected based on selection in Combobox : " + COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                // Finish setup port
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.BaudRate = baudrate;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
                serialPort.Open();
                StatusBox_Status = "'" + serialPort.PortName + "' Port Opened Succesfully";
                IsConnectionSuccesfull = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n");
                Debug.WriteLine("Port " + serialPort.PortName + " could not be opened");
                StatusBox_Error = "Port " + serialPort.PortName + " could not be opened";

                CloseSerialPort();
                IsConnectionSuccesfull = false;
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

            // Multiple (Arduino's) with the correct VID and PID found
            if (comports.Count >= 1)
            {
                String availableComports = "Available COM ports in Windows Registery: ";
                foreach (String s in comports)
                {
                    availableComports += (s) + ", ";
                }
                StatusBox_Status = availableComports.Remove(availableComports.Length - 2);

                ret_port = "";
                COMSelectorWindow COMSelector = new COMSelectorWindow(comports, this);
                COMSelector.ShowDialog();

                return ret_port;
            }
            else if (comports.Count == 1)
            {
                return comports[0].ToString();
            }
            else
            {
                StatusBox_Error = "Fault in autofinding COM ports. No COM found (not in Registery and not in the windowsQuery)";
                StatusBox_Error = "Fault in connection";
                return "COM";
            }
        }
        private static string AutodetectArduinoPort()
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


        // Filter output/input. Remove extra identifier characters
        public string ExtractInput(string input)
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
                StatusBox_Error = "Fault in communication. Could not extract message from input message. Message: (" + input + ")";
                return null;
            }
        }
        public string ExtractErrorMessage(string errorMessage)
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
                StatusBox_Error = "Fault in communication. Could not extract message from Error message. Message: (" + errorMessage + ")";
                return null;
            }
        }
        public string ExtractStatusMessage(string status)
        {
            char[] inputArray = status.ToCharArray();
            int begin = 0, end = 0;
            bool foundBeginning = false;
            for (int i = 0; i < inputArray.Length - 1; i++)
            {
                if (inputArray[i] == '#' && inputArray[i + 1] == '#' && !foundBeginning)
                {
                    begin = i + 1;
                    foundBeginning = true;
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
                StatusBox_Error = "Fault in communication. Could not extract message from Status message. Message: (" + status + ")";
                return null;
            }
        }
        public string ExtractRegistersMessage(string registersMessage)
        {
            char[] registerArray = registersMessage.ToCharArray();
            int begin = 0, end = 0;
            bool foundBeginning = false;
            for (int i = 0; i < registerArray.Length - 1; i++)
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
                StatusBox_Error = "Fault in communication. Could not extract message from Register message. Message: (" + registersMessage + ")";
                return null;
            }
            else
            {
                registersMessage = registersMessage.Substring(begin + 1, (end - begin - 1));
                if (registersMessage.Contains("::"))
                {
                    string[] splitUpRegistersTextBox = registersMessage.Split("_");
                    string output = "";
                    foreach (String str in splitUpRegistersTextBox)
                    {
                        output += str + "\r";
                    }
                    return output;
                }
                else return registersMessage;
            }
        }
    }
}

