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
        DispatcherTimer RefreshCurrentMeasure;
        public void SetupPeriodicStatusses()
        {
            // Every 1s - Measure Current
            RefreshCurrentMeasure = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            MeasuredCurrentPeriodResult.Text = "...";
            RefreshCurrentMeasure.Tick += UpdateMeasuredCurrent;

            RefreshCurrentMeasure.Start();
        }
        
        // Current is updated every 1s (based on the DispatchTimer)
        private void UpdateMeasuredCurrent(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                MeasuredCurrentPeriodResult.Text = MeasureCurrent();
            }
            else if (IsConnectionSuccesfull) IsConnectionSuccesfull = false;
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
                if(ToggleTab_Status.IsChecked == true) TabController.SelectedIndex = 3;
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
                    if(AutoScroll_Commands.IsChecked == true) CommandStatusBox.ScrollToEnd();
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
                    RegistersTextBox.AppendText("[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r");
                    if (AutoScroll_Register.IsChecked == true) RegistersTextBox.ScrollToEnd();
                });
            }
        }
        public string StatusBox_Status
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    Debug.WriteLine(value);
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                    if (AutoScroll_Status.IsChecked == true) Status.ScrollToEnd();
                });
            }
        }
        public string StatusBox_Error
        {
            set
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    Debug.WriteLine(value);
                    if (ToggleTab_Status.IsChecked == true) TabController.SelectedIndex = 3;
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    if (AutoScroll_Status.IsChecked == true) Status.ScrollToEnd();
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

            MeasuredValue = "";
        }
    }
}
