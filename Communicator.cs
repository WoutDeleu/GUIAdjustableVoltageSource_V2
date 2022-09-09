using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

public class Communicator
{
    private string port = "COM5";
    private int baudrate = 115200;
    public SerialPort serialPort;
    public enum Functions
    {
        TOGGLE_LED = 1,
        PUT_VOLTAGE = 2,
        CONNECT_TO_GROUND = 3,
        CONNECT_TO_BUS = 4,
        MEASURE_VOLTAGE = 5,
        MEASURE_CURRENT = 6,
        CHANGE_BOARDNUMBER = 7,
        GET_BOARDNUMBER = 8,
        DISCONNECT_VOLTAGE = 9,
        RESET = 10
    }
    public Communicator()
    {
        serialPort = new SerialPort(port);
        serialPort.PortName = port;
        serialPort.BaudRate = baudrate;
    }

    public void writeSerialPort(string data)
    {
        try
        {
            if (serialPort.IsOpen)
            {
                Thread.Sleep(50);
                serialPort.WriteLine(data);
                Debug.WriteLine("Data to send: " + data);
                Thread.Sleep(50);
            }
            else
            {
                Debug.WriteLine("closed");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    public void initSerialPort()
    {
        try
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();

                if (!serialPort.IsOpen)
                {
                    Debug.WriteLine("'" + serialPort.PortName + "' Port Closed\n");
                }
            }
            
            //// selectieboxjes
            ////if ((string)cmbComPort.SelectedItem != "disconnect")
            //if(true)
            //{
            //    if(true)
            //    //if ((string)cmbComPort.SelectedItem == "auto")
            //    {
            //        if (false)
            //        { }
            //        else if (Regex.IsMatch(AutodetectArduinoPort(), @"\bCOM\d+\b"))
            //        {
            //            serialPort.PortName = AutodetectArduinoPort();
            //        }
            //        else if (Regex.IsMatch(ComPortName("2341", "003D"), @"\bCOM\d+\b"))
            //        {
            //            serialPort.PortName = ComPortName("2341", "003D");
            //        }
            //        else if (Regex.IsMatch(ComPortName("2A03", "003D"), @"\bCOM\d+\b"))
            //        {
            //            serialPort.PortName = ComPortName("2A03", "003D");
            //        }
            //        else
            //        {
            //            Debug.WriteLine("Communicator Connection Problem!");
            //        }
            //    }
            //    else
            //    {
                    serialPort.PortName = port;
                //}

            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.BaudRate = 115200;
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            serialPort.Open();
            Debug.WriteLine("'" + serialPort.PortName + "' Port Opened Succesfully\n");
            //}
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n");
        }
    }
    void closeSerialPort(string port)
    {
        try
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            serialPort.PortName = port;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.BaudRate = 115200;
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            serialPort.Open();
            Debug.WriteLine("'" + serialPort.PortName + "' Port Opened Succesfully\n");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n");
        }
    }

    public void closeSerialPort()
    {
        serialPort.Open();
        serialPort.DiscardOutBuffer();
        serialPort.DiscardInBuffer();

        Thread.Sleep(500);

        serialPort.Dispose();
        serialPort.Close();
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

                Thread.Sleep(100);
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
    //string ComPortName(String VID, String PID)
    //{
    //    String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
    //    Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
    //    List<string> comports = new List<string>();

    //    RegistryKey rk1 = Registry.LocalMachine;
    //    RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

    //    foreach (String s3 in rk2.GetSubKeyNames())
    //    {
    //        RegistryKey rk3 = rk2.OpenSubKey(s3);
    //        foreach (String s in rk3.GetSubKeyNames())
    //        {
    //            //UpdateRichTextBox(s + "\n");
    //            if (_rx.Match(s).Success)
    //            {
    //                RegistryKey rk4 = rk3.OpenSubKey(s);
    //                foreach (String s2 in rk4.GetSubKeyNames())
    //                {
    //                    //UpdateRichTextBox(s2 + "\n");
    //                    RegistryKey rk5 = rk4.OpenSubKey(s2);
    //                    RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
    //                    comports.Add((string)rk6.GetValue("PortName"));
    //                }
    //            }
    //        }
    //    }
    //    //UpdateRichTextBox("Comports.count: " + comports.Count.ToString() + "\n");
    //    if (comports.Count >= 1)
    //    {
    //        string[] portNames = SerialPort.GetPortNames();

    //        foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
    //        {
    //            Console.WriteLine(string.Format("{0} – {1}", comPort.Name, comPort.Description));

    //            if (comPort.Description.Contains("Arduino Due Programming"))
    //            {
    //                return comPort.Name;
    //            }
    //            else if (comPort.Description.Contains("Arduino Due ("))
    //            {
    //                Debug.WriteLine("Arduino connected to the Wrong Micro USB connector!");
    //                throw new Exception("Arduino connected to the Wrong Micro USB connector!\n");
    //            }
    //            else if (comPort.Description == "")
    //            {
    //                foreach (string portName in portNames)
    //                {
    //                    if (comports.Contains(portName))
    //                    {
    //                        return portName;
    //                    }
    //                }
    //            }
    //        }

    //        return "COM";
    //    }
    //    else if (comports.Count == 1)
    //    {
    //        return comports[0].ToString();
    //    }
    //    else
    //    {
    //        return "COM";
    //    }
    //}
    //private string AutodetectArduinoPort()
    //{
    //    ManagementScope connectionScope = new ManagementScope();
    //    SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
    //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

    //    try
    //    {
    //        foreach (ManagementObject item in searcher.Get())
    //        {
    //            string desc = item["Description"].ToString();
    //            string deviceId = item["DeviceID"].ToString();

    //            if (desc == "Arduino Due")
    //            {
    //                Debug.WriteLine("Wrong Micro USB connector!");

    //                throw new Exception("Arduino connected to the Wrong Micro USB connector!\n");
    //            }
    //            else if (desc.Contains("Arduino Due Programming Port"))
    //            {
    //                return deviceId;
    //            }
    //        }
    //        return "";
    //    }
    //    catch (ManagementException e)
    //    {
    //        /* Do Nothing */
    //    }
    //    return "";
    //}
}

