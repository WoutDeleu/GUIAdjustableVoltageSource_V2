using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        DispatcherTimer RefreshArduinoStatus;
        DispatcherTimer RefreshCurrentMeasure;
        public void SetupPeriodicStatusses()
        {
            // Every 8s - Measure Current
            RefreshArduinoStatus = new()
            {
                Interval = TimeSpan.FromSeconds(8)
            };
            RefreshArduinoStatus.Tick += PingArduino;

            // Every 30s - Measure Current
            RefreshCurrentMeasure = new()
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            MeasuredCurrentPeriodResult.Text = "...";
            RefreshCurrentMeasure.Tick += UpdateMeasuredCurrent;

            RefreshCurrentMeasure.Start();
            RefreshArduinoStatus.Start();
        }
        
        // Consistently check the connection with the Arduino
        private void PingArduino(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                string message = "";
                Stopwatch pingTimer = new();
                pingTimer.Start();
                bool connected = false;

                WriteSerialPort((int)BoardFunctions.PING + ";");
                while (!connected)
                {
                    if (pingTimer.ElapsedMilliseconds >= 3000) break;
                    message += serialPort.ReadExisting();
                    if (message.Contains("PING_PING_PING"))
                    {
                        IsConnectionSuccesfull = true;
                        connected = true;
                    }
                }
            }
            else if(IsConnectionSuccesfull) IsConnectionSuccesfull = false; ;
        }
        
        // Current is updated every 30s (based on the DispatchTimer)
        private void UpdateMeasuredCurrent(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                MeasuredCurrentPeriodResult.Text = MeasureCurrent();
            }
        }
        public void UpdateArduinoStatus(bool isConnected)
        {
            HomeTab.IsEnabled = isConnected;
            MeasureTab.IsEnabled = isConnected;
            BoardNumberSettings.IsEnabled = isConnected;

            // Bindings and default values
            IsGndUpdated = isConnected;
            IsBusUpdated = isConnected;

            labelCurrentCOM.Text = CurrentCOMPort;

            if (isConnected)
            {
                ArduinoStatusLabel.Text = "Connected";
                ArduinoStatusBar.Background = Brushes.LightGreen;
            }
            else
            {
                TabController.SelectedIndex = 3;
                ArduinoStatusLabel.Text = "Not Connected";
                ArduinoStatusBar.Background = BrushFromHex("#FFFBFB7A");
            }
        }
        
        // Controllers for statusboxes
        // Remark: first element in boxes are timestamps since start application
        // After 1000 seconds, the timer resets
        public string CommandBox
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    CommandStatusBox.AppendText("[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r");
                    CommandStatusBox.ScrollToEnd();
                });
            }
        }
        public string RegisterBox
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    RegistersTextBox.AppendText("[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\n");
                    RegistersTextBox.ScrollToEnd();
                });
            }
        }
        public string StatusBox_Status
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                    Status.ScrollToEnd();
                });
            }
        }
        public string StatusBox_Error
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    TabController.SelectedIndex = 3;
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    Status.ScrollToEnd();
                });
            }
        }
        public void ClearTextboxes()
        {
            // Clear textboxes logs
            CommandStatusBox.SelectAll();
            CommandStatusBox.Selection.Text = "";

            Status.SelectAll();
            Status.Selection.Text = "";

            RegistersTextBox.SelectAll();
            RegistersTextBox.Selection.Text = "";
        }
    }
}
