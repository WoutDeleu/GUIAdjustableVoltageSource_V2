using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Threading;

public class SerialCommunication
{ 
    public static string port = "COM5";
	public enum Functions
	{
        TOGGLE_LED = 0,
        PUT_VOLTAGE = 1,
        CONNECT_TO_GROUND = 2,
        CONNECT_TO_BUS = 3,
        MEASURE_VOLTAGE = 4,
        MEASURE_CURRENT = 5,
        CHANGE_BOARDNUMBER = 6,
        GET_BOARDNUMBER = 7,
    }
	public SerialCommunication()
	{
	}

	public static void writeSerialPort(string data, SerialPort serialPort)
	{
		try
		{
			if(serialPort.IsOpen)
			{
				Thread.Sleep(50);
				serialPort.WriteLine(data);
				Thread.Sleep(50);
			}
		}
		catch(Exception ex)
		{
			// Status
		}
	}
    //Nog niet functionall...
	public static void initSerialPort(SerialPort serialPort)
	{
		try
		{
			if(serialPort.IsOpen)
			{
				serialPort.Close();
				if(!serialPort.IsOpen)
				{
					// Status
				}
			}
			// Autodetect? zie Jeroen
            serialPort.PortName = port;
			
			
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.BaudRate = 115200;
			// serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceivedHandler);
            // serialPort.Open();
			//Status
		}
		catch (Exception ex)
		{
			// Status
		}
	}

	// Klopt naam? Foutje in naam functie voorbeeld Jeroen?
    public static void openSerialPort(SerialPort serialPort)
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
			// serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceivedHandler);
            serialPort.Open();
			// Status
        }
        catch (Exception ex)
        {
			// Status
        }
    }

    public static void closeSerialPort(SerialPort serialPort)
	{
		serialPort.Open();
		serialPort.DiscardOutBuffer();
		serialPort.DiscardInBuffer();
			
		Thread.Sleep(500);
			
		serialPort.Dispose();
		serialPort.Close();
	}

/*        private string AutodetectArduinoPort()
    {
        ManagementScope connectionScope = new ManagementScope();
        SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

        try
        {
            foreach (ManagementObject item in searcher.Get())
            {
                string desc = item["Description"].ToString();
                string deviceId = item["DeviceID"].ToString();

                if (desc == "Arduino Due")
                {

                    throw new Exception("Arduino connected to the Wrong Micro USB connector!\n");
                }
                else if (desc.Contains("Arduino Due Programming Port"))
                {
                    return deviceId;
                }
            }
            return "";
        }
        catch (ManagementException e)
        {
            *//* Do Nothing *//*
        }
        return "";
    }
*/
}

