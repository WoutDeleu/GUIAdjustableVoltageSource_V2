using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Controllers for input for
        // Remark: first element in boxes are timestamps since start application
        // After 1000 seconds, the timer resets
        public string CommandBox
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    if(stopwatch.Elapsed.TotalSeconds>1000) stopwatch.Restart();
                    CommandInterface.AppendText("[" + stopwatch.Elapsed.TotalSeconds +"] "+ value + "\r");
                });
            }
        }
        public string RegisterBox
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (stopwatch.Elapsed.TotalSeconds > 1000) stopwatch.Restart();
                    Registers.AppendText("[" + stopwatch.Elapsed.TotalSeconds + "] " + value);
                });
            }
        }
        public string StatusBox_Status
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (stopwatch.Elapsed.TotalSeconds > 1000) stopwatch.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + stopwatch.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                });
            }
        }
        public string StatusBox_Error
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (stopwatch.Elapsed.TotalSeconds > 1000) stopwatch.Restart();
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = "[" + stopwatch.Elapsed.TotalSeconds + "] " + value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                });
            }
        }
    }
}
