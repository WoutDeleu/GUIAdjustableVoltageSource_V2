using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
            switch (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Others":
                    newComPort.IsEnabled = true;
                    break;
                default:
                    newComPort.IsEnabled = false;
                    break;
            }
        }
    }
}
