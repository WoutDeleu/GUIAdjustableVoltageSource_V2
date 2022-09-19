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
			if (Communicator.serialPort.IsOpen)
			{
				string message = "";
				Stopwatch pingTimer = new();
				pingTimer.Start();
				bool connected = false;

				Communicator.WriteSerialPort((int)Communicator.Functions.PING + ";");
				while (!connected)
				{
					if (pingTimer.ElapsedMilliseconds >= 1000) break;
					message += Communicator.serialPort.ReadExisting();
					if (message.Contains("8888")) connected = true;
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
		// Current is updated every 30s (based on the DispatchTimer)
		private void UpdateMeasuredCurrent(object sender, EventArgs e)
		{
			if (Communicator.serialPort.IsOpen)
			{
				MeasuredCurrentPeriodResult.Text = MeasureCurrent();
			}
		}
	}
}
