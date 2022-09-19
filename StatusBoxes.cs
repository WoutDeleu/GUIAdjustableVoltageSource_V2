using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        // Controllers for statusboxes
        // Remark: first element in boxes are timestamps since start application
        // After 1000 seconds, the timer resets
        public string CommandBox
        {
            set
            {
                this.Dispatcher.Invoke(() =>
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
                this.Dispatcher.Invoke(() =>
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
                this.Dispatcher.Invoke(() =>
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
                this.Dispatcher.Invoke(() =>
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

        // Update Statusboxes and disable functions based on connectionstatus
        private void SuccesfullCommunication()
        {
            HomeTab.IsEnabled = true;
            MeasureTab.IsEnabled = true;
            BoardNumberSettings.IsEnabled = true;

            // Bindings and default values
            IsGndUpdated = true;
            IsBusUpdated = true;
        }
        private void UnSuccesfullCommunication()
        {
            HomeTab.IsEnabled = false;
            MeasureTab.IsEnabled = false;
            BoardNumberSettings.IsEnabled = false;
            TabController.SelectedIndex = 3;
        }
    }
}
