using System.Windows;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Data;
using System.Threading;

namespace AdjustableVoltageSource
{
	public partial class MainWindow
	{
		// Set correct voltage on arduino
        private void PutVoltage(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			string voltagestr = SetVoltageTextBox.Text.Replace(".", ",");
			if (IsValidVoltage(voltagestr))
            {
                SetVoltageTextBox.BorderBrush = BrushFromHex("#FFABADB3");
                SetVoltageTextBox.Background = Brushes.White;
                Voltage = Convert.ToDouble(voltagestr);
				WriteSerialPort((int)BoardFunctions.PUT_VOLTAGE + "," + Voltage + ";");
				// StatusBox_Status = "Set Voltage to " + Voltage + ".";
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
			WriteSerialPort((int)BoardFunctions.DISCONNECT_VOLTAGE + ";");
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
				WriteSerialPort((int)BoardFunctions.CHANGE_BOARDNUMBER + "," + BoardNumber + ";");
			}
			else
			{
				StatusBox_Error = "Fault while setting BoardNumber. Fault in format.";
			}
		}
		// Fetch BoardNumber stored on the arduino code to see what boardNr it will to connect to
		private void UpdateBoardNumber()
		{

			WriteSerialPort((int)BoardFunctions.GET_BOARDNUMBER + ";");

			string input = "";
			while (serialPort.BytesToRead != 0)
			{
				input += serialPort.ReadExisting();
			}
			string nr = ExtractInput(input);
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
			if (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Measure Current")
			{
				MeasuredValue = MeasureCurrent();
                MeasuredCurrentPeriodResult.Text = MeasuredValue;

            }
			else MeasuredValue = MeasureVoltage();
		}
		private string MeasureVoltage()
		{
			try
			{
				if (serialPort.IsOpen)
				{
					MeasureVoltageChannel();

					String input = "";

					while (serialPort.BytesToRead != 0)
					{
						input += serialPort.ReadExisting();
					}
					Debug.WriteLine(input);
					string voltage = ExtractInput(input).Replace(".", ",");
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

			WriteSerialPort((int)BoardFunctions.MEASURE_CURRENT + ";");

			String input = "";
            Thread.Sleep(200);

            while (serialPort.BytesToRead != 0)
            {
                input += serialPort.ReadExisting();
            }
			Debug.WriteLine(input);
			string current = "";
            if (input != "") current = ExtractInput(input).Replace(".", ",");
            if (double.TryParse(current, out double current_out))
            {
                StatusBox_Status = "Measured Current: " + current_out;
                return current_out + " A";
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

			WriteSerialPort((int)BoardFunctions.MEASURE_VOLTAGE + "," + channel + ";");
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
						newCOMPortTextBox.BorderBrush = BrushFromHex("#FFABADB3");
						newCOMPortTextBox.Background = Brushes.White;
						ResetBasedOnCOM();
					}
					else
                    {
                        newCOMPortTextBox.BorderBrush = Brushes.DarkRed;
                        newCOMPortTextBox.Background = Brushes.LightPink;
                        StatusBox_Error = "Fault in format Comport... Format is e.g. 'COM8'";
					}
					break;
                default:
                    newCOMPortTextBox.BorderBrush = BrushFromHex("#FFABADB3");
                    newCOMPortTextBox.Background = Brushes.White;
                    ResetBasedOnCOM();
                    break;
            }
        }
		public void ResetBasedOnCOM()
		{
            if (IsConnectionSuccesfull) Reset();
            else ResetWithClosedPort();
        }
        public void Reset()
        {
            DisconnectAll();
            SetVoltageTextBox.Text = "";
            MeasuredCurrentPeriodResult.Text = "Not Yet Measured";
            WriteSerialPort((int)BoardFunctions.DISCONNECT_VOLTAGE + ";");

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
