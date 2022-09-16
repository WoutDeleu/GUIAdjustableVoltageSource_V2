using System.Windows;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
	public partial class MainWindow
	{
		// Set correct voltage on arduino
		private void PutVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			string voltagestr = VoltageTextBox.Text.Replace(".", ",");
			if (IsValidVoltage(voltagestr))
            {
                VoltageTextBox.BorderBrush = (Brush)bc.ConvertFrom("#FFABADB3");
                VoltageTextBox.Background = Brushes.White;
                voltage = Convert.ToDouble(voltagestr);
				communicator.WriteSerialPort((int)Communicator.Functions.PUT_VOLTAGE + "," + voltage + ";");
				StatusBox_Status = "Set Voltage to " + voltage + ".";
			}
			else
            {
                VoltageTextBox.BorderBrush = Brushes.DarkRed;
                VoltageTextBox.Background = Brushes.LightPink;
                StatusBox_Error = "Invalid Voltage, voltage must be in range [0V...30V] and it must be a number.";
			}
		}

		// disconnect voltage source on arduino
		public void DisconnectVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			VoltageTextBox.Text = "";
			communicator.WriteSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");
			StatusBox_Status = "Disconnect voltageSource";
		}

		// Write the chosen boardNr to the Arduino
		private void ApplyBoardNumber(object sender, RoutedEventArgs e)
		{
			String boardNumberStr = NewBoardNumber.Text;
			if(boardNumberStr == "") { StatusBox_Status = "BoardNumber was not changed"; }
			// Regex: check if boardNr is valid
			else if (Regex.IsMatch(boardNumberStr, @"^\d+$"))
			{
				BoardNumber = Convert.ToInt32(boardNumberStr);
				communicator.WriteSerialPort((int)Communicator.Functions.CHANGE_BOARDNUMBER + "," + BoardNumber + ";");
			}
			else
			{
				StatusBox_Error = "Fault while setting BoardNumber. Fault in format.";
			}
		}
		// Fetch BoardNumber stored on the arduino code to see what boardNr it will to connect to
		private void GetBoardNumberArduino()
		{
			int boardNumber;

			communicator.WriteSerialPort((int)Communicator.Functions.GET_BOARDNUMBER + ";");

			string input = "";
			while (communicator.serialPort.BytesToRead != 0)
			{
				input += communicator.serialPort.ReadExisting();
			}
			string nr = Communicator.ExtractInput(input, this);
			if (int.TryParse(nr, out boardNumber)) 
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
			if (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Measure Current")
			{
				double current_out;
				communicator.WriteSerialPort((int)Communicator.Functions.MEASURE_CURRENT + ";");

				String input = "";

				while (communicator.serialPort.BytesToRead != 0)
				{
					input += communicator.serialPort.ReadExisting();
				}
				string current = Communicator.ExtractInput(input, this).Replace(".", ",");
				if (double.TryParse(current, out current_out))
				{
					MeasuredValue = current_out + " A";
					StatusBox_Status = "Measured Current: " + current_out;
				}
				else
				{
					StatusBox_Error = "Fault in measure format";
					MeasuredValue = "FAULT";
				}
			}
			else
			{
				double voltage_out;
				MeasureVoltageChannel();

				String input = "";

				while (communicator.serialPort.BytesToRead != 0)
				{
					input += communicator.serialPort.ReadExisting();
				}
				Debug.WriteLine(input);
				string voltage = Communicator.ExtractInput(input, this).Replace(".", ",");
				if (double.TryParse(voltage, out voltage_out))
				{
					StatusBox_Status = "Measured Voltage: " + voltage_out;
					MeasuredValue = voltage_out + " V";
				}
				else
				{
					StatusBox_Error = "Fault in measure format (received)...";
					MeasuredValue = "FAULT";
				}
			}
		}
		// Formatting messages based on the selection of the measuring channel
		private void MeasureVoltageChannel()
		{
			string channel = "";
			if (ch1_radiobutton.IsChecked == true) channel = "1";
			else if (ch2_radiobutton.IsChecked == true) channel = "2";
			else if (ch3_radiobutton.IsChecked == true) channel = "3";
			else if (ch4_radiobutton.IsChecked == true) channel = "4";
			else if (ch5_radiobutton.IsChecked == true) channel = "5";
			else if (ch6_radiobutton.IsChecked == true) channel = "6";
			else if (ch7_radiobutton.IsChecked == true) channel = "7";
			else if (ch8_radiobutton.IsChecked == true) channel = "8";
			else if (ch9_radiobutton.IsChecked == true) channel = "9";
			else if (ch10_radiobutton.IsChecked == true) channel = "10";
			else if (ch11_radiobutton.IsChecked == true) channel = "11";
			else if (ch12_radiobutton.IsChecked == true) channel = "12";
			else if (ch13_radiobutton.IsChecked == true) channel = "13";
			else if (ch14_radiobutton.IsChecked == true) channel = "14";
			else if (ch15_radiobutton.IsChecked == true) channel = "15";
			else if (ch16_radiobutton.IsChecked == true) channel = "16";

			communicator.WriteSerialPort((int)Communicator.Functions.MEASURE_VOLTAGE + "," + channel + ";");
		}

		// Change the port used to interact with the Arduino
		private void ChangeCOMPort(object sender, RoutedEventArgs e)
		{
			string selectedPort = ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            switch (selectedPort)
            {
				case "Others":
					if (IsValidCOMPort(newComPortTextBox.Text))
					{
						newComPortTextBox.BorderBrush = (Brush)bc.ConvertFrom("#FFABADB3");
						newComPortTextBox.Background = Brushes.White;
						Reset();
					}
					else
                    {
                        newComPortTextBox.BorderBrush = Brushes.DarkRed;
                        newComPortTextBox.Background = Brushes.LightPink;
                        StatusBox_Error = "Fault in format Comport... Format is e.g. 'COM8'";
					}
					break;
                default:
                    newComPortTextBox.BorderBrush = (Brush)bc.ConvertFrom("#FFABADB3");
                    newComPortTextBox.Background = Brushes.White;
                    Reset();
                    break;
            }
        }

        public void Reset()
        {
            DisconnectAll();
            VoltageTextBox.Text = "";
            communicator.WriteSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");

            stopwatch = Stopwatch.StartNew();

            InitializeMainWindow();
        }

		public void ResetWithClosedPort()
		{
            DisconnectAllWithClosedPort();
			VoltageTextBox.Text = "";

			stopwatch = Stopwatch.StartNew();

			InitializeMainWindow();
		}

	}
}
