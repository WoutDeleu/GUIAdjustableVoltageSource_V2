using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

public class Tierce
{ 
    private string port = "COM4";
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
    }
	public Tierce()
	{
        serialPort = new SerialPort(port);
        serialPort.PortName = port;
        serialPort.BaudRate = baudrate;
	}

	public void writeSerialPort(string data)
	{
		try
		{
			if(serialPort.IsOpen)
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
		catch(Exception ex)
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
            /*
            if ((string)cmbComPort.SelectedItem != "disconnect")
            {
                if ((string)cmbComPort.SelectedItem == "auto")
                {
                    if (false)
                    { }
                    else if (Regex.IsMatch(AutodetectArduinoPort(), @"\bCOM\d+\b"))
                    {
                        serialPort.PortName = AutodetectArduinoPort();
                    }
                    else if (Regex.IsMatch(ComPortName("2341", "003D"), @"\bCOM\d+\b"))
                    {
                        serialPort.PortName = ComPortName("2341", "003D");
                    }
                    else if (Regex.IsMatch(ComPortName("2A03", "003D"), @"\bCOM\d+\b"))
                    {
                        serialPort.PortName = ComPortName("2A03", "003D");
                    }
                    else
                    {
                        statusStrip.Items["connectionStatus"].Text = "Tierce Connection Problem!";
                        statusStrip.BackColor = Color.Orange;
                    }
                }
                else
                {
            */
                    serialPort.PortName = port;
                // }

                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.BaudRate = 115200;
                this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                serialPort.Open();
                Debug.WriteLine("'" + serialPort.PortName + "' Port Opened Succesfully\n");
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

}

