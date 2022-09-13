using System.Windows;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

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
				voltage = Convert.ToDouble(voltagestr);
				communicator.writeSerialPort((int)Communicator.Functions.PUT_VOLTAGE + "," + voltage + ";");
				StatusBox_Status = "Set Voltage to " + voltage + ".";
			}
			else StatusBox_Error = "Invalid Voltage, voltage must be in range [0V...30V] and it must be a number.";
		}

		// disconnect voltage source on arduino
		public void DisconnectVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			VoltageTextBox.Text = "";
			communicator.writeSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");
			StatusBox_Status = "Disconnect voltageSource";
		}

		// Write the chosen boardNr to the Arduino
		private void ApplyBoardNumber(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			String boardNumberStr = NewBoardNumber.Text;
			// Regex: check if boardNr is valid
			if (Regex.IsMatch(boardNumberStr, @"^\d+$"))
			{
				BoardNumber = Convert.ToInt32(boardNumberStr);
				communicator.writeSerialPort((int)Communicator.Functions.CHANGE_BOARDNUMBER + "," + BoardNumber + ";");
			}
			else
			{
				StatusBox_Error = "Fault setting BoardNumber => Fault in format.";
			}
		}
		// Fetch BoardNumber stored on the arduino code to see what boardNr it will to connect to
		private void GetBoardNumberArduino()
		{
			int boardNumber;

			communicator.writeSerialPort((int)Communicator.Functions.GET_BOARDNUMBER + ";");

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
				communicator.writeSerialPort((int)Communicator.Functions.MEASURE_CURRENT + ";");

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
			if (ch1.IsChecked == true) channel = "1";
			else if (ch2.IsChecked == true) channel = "2";
			else if (ch3.IsChecked == true) channel = "3";
			else if (ch4.IsChecked == true) channel = "4";
			else if (ch5.IsChecked == true) channel = "5";
			else if (ch6.IsChecked == true) channel = "6";
			else if (ch7.IsChecked == true) channel = "7";
			else if (ch8.IsChecked == true) channel = "8";
			else if (ch9.IsChecked == true) channel = "9";
			else if (ch10.IsChecked == true) channel = "10";
			else if (ch11.IsChecked == true) channel = "11";
			else if (ch12.IsChecked == true) channel = "12";
			else if (ch13.IsChecked == true) channel = "13";
			else if (ch14.IsChecked == true) channel = "14";
			else if (ch15.IsChecked == true) channel = "15";
			else if (ch16.IsChecked == true) channel = "16";

			communicator.writeSerialPort((int)Communicator.Functions.MEASURE_VOLTAGE + "," + channel + ";");
		}
	}
}
