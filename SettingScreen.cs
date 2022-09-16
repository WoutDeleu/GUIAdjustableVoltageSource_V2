using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _boardNumber;
        public int BoardNumber
        {
            get { return _boardNumber; }
            set
            {
                if (value != _boardNumber)
                {
                    _boardNumber = value;
                    OnPropertyChanged("BoardNumber");
                }
            }
        }
        public string CurrentComPort
        {
            get { return communicator.serialPort.PortName; }
        }
        
        // Handles enabling/disabling textbox to select comport
        // Based on closing dropdown menu
        private bool handledPort = true;
        private void selectCom_DropDownClosed(object sender, EventArgs e)
        {
            if (handledPort) HandleCom();
            handledPort = true;
        }
        private void selectCom_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handledPort = !cmb.IsDropDownOpen;
            HandleCom();
        }
        private void HandleCom()
        {
            switch (ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Others":
                    newComPortTextBox.IsEnabled = true;
                    newComPortTextBox.Background = Brushes.White;
                    ManualComport.Foreground = Brushes.Black;
                    break;
                default:
                    newComPortTextBox.IsEnabled = false;
                    newComPortTextBox.Background = Brushes.Gray;
                    ManualComport.Foreground = Brushes.Gray;
                    break;
            }
        }
    }
}
