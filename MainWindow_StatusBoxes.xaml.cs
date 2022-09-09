using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string CommandBox
        {
            set { CommandInterface.AppendText(value + "\r"); }
        }
        public string StatusBox_Status
        {
            set { Status.AppendText(value + "\r"); }
        }
        public string StatusBox_Error
        {
            set 
            {
                Status.AppendText(value + "\r");
            }
        }
        public string RegisterBox
        {
            set { Registers.AppendText(value + "\r"); }
        }
    }
}
