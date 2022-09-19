using System.Windows;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.IO.Ports;
using System.Windows.Data;

namespace AdjustableVoltageSource
{
	public partial class MainWindow
	{
		// Establish communication with the arduino
        private void InitializeCommunication()
        {
            ClearTextboxes();

            Communicator.InitSerialPort();
            labelCurrentCOM.Text = CurrentCOMPort;

            if (!Communicator.isConnectionSuccesfull)
            {

                currentCOM.SetBinding(ContentProperty, new Binding("currentCOMPort"));

                ArduinoStatusLabel.Text = "Not Connected";
                ArduinoStatusBar.Background = BrushFromHex("#FFFBFB7A");
                UnSuccesfullCommunication();

            }
            else
            {
                try
                {
                    // Loop until communication is established
                    string message = "";
                    bool started = false, finished = false;
                    Stopwatch startupTimer = new();
                    startupTimer.Start();
                    while (!started)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 5000) throw new Exception("TIMEOUT: can't START setup communication Arduino.");
                        message += Communicator.serialPort.ReadExisting();
                        if (message.Contains("##Setup Arduino##")) started = true;
                    }
                    StatusBox_Status = "Setup Arduino started";
                    while (!finished && startupTimer.ElapsedMilliseconds < 5000)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 10000) throw new Exception("TIMEOUT: can't FINISH setup communication Arduino.");
                        message += Communicator.serialPort.ReadExisting();
                        if (message.Contains("##Setup Complete##")) finished = true;
                    }
                    StatusBox_Status = "Setup Arduino finished";
                    SuccesfullCommunication();

                    ArduinoStatusLabel.Text = "Connected";
                    ArduinoStatusBar.Background = Brushes.LightGreen;

                    GetBoardNumberArduino();
                    DataContext = this;
                }
                catch (Exception ex)
                {
                    StatusBox_Error = ex.Message.ToString() + " Check if communication is correct and if Arduino is available.";

                    UnSuccesfullCommunication();

                    StatusBox_Error = ("Port " + Communicator.serialPort.PortName + " could not be opened");
                    Communicator.CloseSerialPort();
                    Communicator.isConnectionSuccesfull = false;

                    ArduinoStatusLabel.Text = "Not Connected";
                    ArduinoStatusBar.Background = BrushFromHex("#FFFBFB7A");
                }
            }
        }
        
		// Set correct voltage on arduino
        private void PutVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			string voltagestr = SetVoltageTextBox.Text.Replace(".", ",");
			if (IsValidVoltage(voltagestr))
            {
                SetVoltageTextBox.BorderBrush = (Brush)Bc.ConvertFrom("#FFABADB3");
                SetVoltageTextBox.Background = Brushes.White;
                Voltage = Convert.ToDouble(voltagestr);
				Communicator.WriteSerialPort((int)Communicator.Functions.PUT_VOLTAGE + "," + Voltage + ";");
				StatusBox_Status = "Set Voltage to " + Voltage + ".";
			}
			else
            {
                SetVoltageTextBox.BorderBrush = Brushes.DarkRed;
                SetVoltageTextBox.Background = Brushes.LightPink;
                StatusBox_Error = "Invalid Voltage, voltage must be in range [0V...30V] and it must be a number.";
			}
		}

		// disconnect voltage source on arduino
		public void DisconnectVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			SetVoltageTextBox.Text = "";
			Communicator.WriteSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");
			StatusBox_Status = "Disconnect voltageSource";
		}

		// Write the chosen boardNr to the Arduino
		private void ApplyBoardNumber(object sender, RoutedEventArgs e)
		{
			String boardNumberStr = NewBoardNumber.Text;
			// Regex: check if boardNr is valid
			if (Regex.IsMatch(boardNumberStr, @"^\d+$"))
			{
				BoardNumber = Convert.ToInt32(boardNumberStr);
				Communicator.WriteSerialPort((int)Communicator.Functions.CHANGE_BOARDNUMBER + "," + BoardNumber + ";");
			}
			else
			{
				StatusBox_Error = "Fault while setting BoardNumber. Fault in format.";
			}
		}
		// Fetch BoardNumber stored on the arduino code to see what boardNr it will to connect to
		private void GetBoardNumberArduino()
		{

			Communicator.WriteSerialPort((int)Communicator.Functions.GET_BOARDNUMBER + ";");

			string input = "";
			while (Communicator.serialPort.BytesToRead != 0)
			{
				input += Communicator.serialPort.ReadExisting();
			}
			string nr = Communicator.ExtractInput(input, this);
			if (int.TryParse(nr, out int boardNumber)) 
			{
				StatusBox_Status = "Boardnumber received: " + boardNumber;
				BoardNumber = boardNumber; 
			}
			else
			{
				StatusBox_Error = "Fault in fetching boardNumber...";
				BoardNumber = 999999;
			}
		}

		// Measure the current/voltage (based on the selection in the combobox) from the Arduino
		private void MeasureValue(object sender, RoutedEventArgs e)
		{
			if (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Measure Current") MeasuredValue = MeasureCurrent();
			else MeasuredValue = MeasureVoltage();
		}
		private string MeasureVoltage()
		{
			try
			{
				if (Communicator.serialPort.IsOpen)
				{
					MeasureVoltageChannel();

					String input = "";

					while (Communicator.serialPort.BytesToRead != 0)
					{
						input += Communicator.serialPort.ReadExisting();
					}
					Debug.WriteLine(input);
					string voltage = Communicator.ExtractInput(input, this).Replace(".", ",");
					if (double.TryParse(voltage, out double voltage_out))
					{
						StatusBox_Status = "Measured Voltage: " + voltage_out;
						return voltage_out + " V";
					}
					else
					{
						StatusBox_Error = "Fault in measure format (received)...";
						return "FAULT";
					}
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
				ResetWithClosedPort();
			}
			return "";
        }
		private string MeasureCurrent()
		{

            double current_out;
            Communicator.WriteSerialPort((int)Communicator.Functions.MEASURE_CURRENT + ";");

            String input = "";

            while (Communicator.serialPort.BytesToRead != 0)
            {
                input += Communicator.serialPort.ReadExisting();
            }
            string current = Communicator.ExtractInput(input, this).Replace(".", ",");
            if (double.TryParse(current, out current_out))
            {
                return current_out + " A";
                StatusBox_Status = "Measured Current: " + current_out;
            }
            else
            {
                StatusBox_Error = "Fault in measure format";
                return "FAULT";
            }
        }
		// Formatting messages based on the selection of the measuring channel
		private void MeasureVoltageChannel()
		{
			string channel = "";
			if (MeasureVoltageCh1.IsChecked == true) channel = "1";
			else if (MeasureVoltageCh2.IsChecked == true) channel = "2";
			else if (MeasureVoltageCh3.IsChecked == true) channel = "3";
			else if (MeasureVoltageCh4.IsChecked == true) channel = "4";
			else if (MeasureVoltageCh5.IsChecked == true) channel = "5";
			else if (MeasureVoltageCh6.IsChecked == true) channel = "6";
			else if (MeasureVoltageCh7.IsChecked == true) channel = "7";
			else if (MeasureVoltageCh8.IsChecked == true) channel = "8";
			else if (MeasureVoltageCh9.IsChecked == true) channel = "9";
			else if (MeasureVoltageCh10.IsChecked == true) channel = "10";
			else if (MeasureVoltageCh11.IsChecked == true) channel = "11";
			else if (MeasureVoltageCh12.IsChecked == true) channel = "12";
			else if (MeasureVoltageCh13.IsChecked == true) channel = "13";
			else if (MeasureVoltageCh14.IsChecked == true) channel = "14";
			else if (MeasureVoltageCh15.IsChecked == true) channel = "15";
			else if (MeasureVoltageCh16.IsChecked == true) channel = "16";

			Communicator.WriteSerialPort((int)Communicator.Functions.MEASURE_VOLTAGE + "," + channel + ";");
		}

		// Change the port used to interact with the Arduino
		private void ChangeCOMPort(object sender, RoutedEventArgs e)
		{
			string selectedPort = COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (selectedPort)
            {
				case "Others":
					if (IsValidCOMPort(newCOMPortTextBox.Text))
					{
						newCOMPortTextBox.BorderBrush = (Brush)Bc.ConvertFrom("#FFABADB3");
						newCOMPortTextBox.Background = Brushes.White;
						Reset();
					}
					else
                    {
                        newCOMPortTextBox.BorderBrush = Brushes.DarkRed;
                        newCOMPortTextBox.Background = Brushes.LightPink;
                        StatusBox_Error = "Fault in format Comport... Format is e.g. 'COM8'";
					}
					break;
                default:
                    newCOMPortTextBox.BorderBrush = (Brush)Bc.ConvertFrom("#FFABADB3");
                    newCOMPortTextBox.Background = Brushes.White;
                    Reset();
                    break;
            }
        }

        public void Reset()
        {
            DisconnectAll();
            SetVoltageTextBox.Text = "";
            MeasuredCurrentPeriodResult.Text = "Not Yet Set";
            Communicator.WriteSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");

            AppTimer = Stopwatch.StartNew();

            InitializeCommunication();
        }
		// Reset, but when the COM-port is closed
		// No registers are written
		public void ResetWithClosedPort()
		{
            DisconnectAllWithClosedPort();
			SetVoltageTextBox.Text = "";

			AppTimer = Stopwatch.StartNew();

			InitializeCommunication();
		}
	}
}
