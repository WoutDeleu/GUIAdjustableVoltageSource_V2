using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Windows.Media.Animation;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
		// Consistently check the connection with the Arduino
        private void UpdateArduinoStatus(object sender, EventArgs e)
        {
			if (communicator.serialPort.IsOpen)
			{
				string message = "";
				Stopwatch pingTimer = new Stopwatch();
				pingTimer.Start();
				bool connected = false;

				communicator.WriteSerialPort((int)Communicator.Functions.PING + ";");
				while (!connected)
				{
					if (pingTimer.ElapsedMilliseconds >= 1000) break;
					message += communicator.serialPort.ReadExisting();
					if (message.Contains("PING_PING_PING")) connected = true;
				}
				if (connected)
				{
					ArduinoStatusLabel.Text = "Connected";
					ArduinoStatusBar.Background = Brushes.LightGreen;
				}
				else
				{
					ArduinoStatusLabel.Text = "Not Connected";
					ArduinoStatusBar.Background = BrushFromHex("#FFFBFB7A");
				}

			}
			else
			{
				ArduinoStatusLabel.Text = "Not Connected";
				ArduinoStatusBar.Background = BrushFromHex("#FFFBFB7A");
			}
        }
		private void MeasureCurrentPeriod(object sender, EventArgs e)
		{
			if (communicator.serialPort.IsOpen)
			{
				MeasureCurrentPeriodText.Text = MeasureCurrent();
			}
		}
	}
}
