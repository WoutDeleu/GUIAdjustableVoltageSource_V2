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
        public void SetupPeriodicCurrent()
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
            currentCOM.Content = CurrentCOMPort;

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
                Dispatcher.BeginInvoke(() =>
                {
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    if (MeasureCurrentFilter.IsChecked == false || !HasCurrentRefs_Command(value))
                    {
                        CommandStatusBox.AppendText("[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r");
                        if (AutoScroll_Commands.IsChecked == true) CommandStatusBox.ScrollToEnd();
                    }
                });
            }
        }
        public string RegisterBox
        {
            set
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    if (MeasureCurrentFilter.IsChecked == false || !HasCurrentRefs_Register(value))
                    {
                        RegistersTextBox.AppendText("[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r");
                        if (AutoScroll_Register.IsChecked == true) RegistersTextBox.ScrollToEnd();
                    }
                });
            }
        }
        public string StatusBox_Status
        {
            set
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Debug.WriteLine(value);
                    if (AppTimer.Elapsed.TotalSeconds > 1000) AppTimer.Restart();
                    // Filter out messages containing info about Current
                    if (MeasureCurrentFilter.IsChecked == false || !HasCurrentRefs_Status(value))
                    {
                        TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                        tr.Text = "[" + AppTimer.Elapsed.TotalSeconds + "] " + value + "\r";
                        tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                        if (AutoScroll_Status.IsChecked == true) Status.ScrollToEnd();
                    }

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
        

        // Check if messages have a relation to Current measurement (Filter current measurement messages)
        public static bool HasCurrentRefs_Status(string Message)
        {
            if (Message.Contains("current measurement")) return true;
            else if (Message.Contains("Measured Current")) return true;
            else return false;
        }
        public static bool HasCurrentRefs_Register(string Message)
        {
            if (Message.Contains("MEASURE REGISTER")) return true;
            else return false;
        }
        public static bool HasCurrentRefs_Command(string Message)
        {
            if (Message.Contains("Measure Current")) return true;
            else return false;
        }

        public void ClearLogs(object sender, EventArgs e)
        {
            ClearTextboxes();
        }
    }
}
