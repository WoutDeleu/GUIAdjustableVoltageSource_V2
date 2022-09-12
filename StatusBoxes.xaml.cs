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
        public string CommandBox
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    CommandInterface.AppendText(value + "\r");
                });
            }
        }
        public string StatusBox_Status
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    Status.AppendText(value + "\r");
                });
            }
        }
        public string StatusBox_Error
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    TextRange tr = new TextRange(Status.Document.ContentEnd, Status.Document.ContentEnd);
                    tr.Text = value + "\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                });
            }
        }
        public string RegisterBox
        {
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    Registers.AppendText(value + "\r\r");
                });
            }
        }
    }
}
