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
        public string CurrentCOMPort
        {
            get { return Communicator.serialPort.PortName; }
        }
        
        // Handles enabling/disabling textbox to select comport
        // Based on closing dropdown menu
        private bool handledPort = true;
        private void selectCOM_DropDownClosed(object sender, EventArgs e)
        {
            if (handledPort) HandleCom();
            handledPort = true;
        }
        private void selectCOM_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handledPort = !cmb.IsDropDownOpen;
            HandleCom();
        }
        private void HandleCom()
        {
            switch (COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Others":
                    newCOMPortTextBox.IsEnabled = true;
                    newCOMPortTextBox.Background = Brushes.White;
                    ManualComport.Foreground = Brushes.Black;
                    break;
                default:
                    newCOMPortTextBox.IsEnabled = false;
                    newCOMPortTextBox.Background = Brushes.Gray;
                    ManualComport.Foreground = Brushes.Gray;
                    break;
            }
        }
    }
}
