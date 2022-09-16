using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _measuredValue;

        private Visibility selectionVisible;
        public Visibility SelectionVisible 
        {
            get
            {
                return selectionVisible;
            }
            set
            {
                selectionVisible = value;
                OnPropertyChanged("SelectionVisible");
            }
        }

        public string MeasuredValue
        {
            get { return _measuredValue; }
            set
            {
                if (value != _measuredValue)
                {
                    _measuredValue = value;
                    OnPropertyChanged("MeasuredValue");
                }
            }
        }

        private bool handledMeasureSelection = true;

        private void selectMeasureFunction_DropDownClosed(object sender, EventArgs e)
        {
            if (handledMeasureSelection) HandleSelectMeasure();
            handledMeasureSelection = true;
        }
        private void selectMeasureFunction_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handledMeasureSelection = !cmb.IsDropDownOpen;
            HandleSelectMeasure();
        }
        
        // Based on the selection of the combobox (measure current/voltage) the selection of channels is invisible/visible
        // Measuring current is only possible on 1 defined port
        // Measuring voltage is possible on all ports which are connected to the bus
        private void HandleSelectMeasure()
        {
            switch (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "Measure Current":
                    SelectionVisible = Visibility.Collapsed;
                    break;
                default:
                    SelectionVisible = Visibility.Visible;
                    break;
            }
        }
        // Sync values selected on the HomeScreen with the ones on the MeasureScreen (enable/disable radiobuttons)
        private void UpdateMeasureBoxes()
        {
            ch1_radiobutton.IsEnabled = ConnectedToBus_1;
            ch2_radiobutton.IsEnabled = ConnectedToBus_2;
            ch3_radiobutton.IsEnabled = ConnectedToBus_3;
            ch4_radiobutton.IsEnabled = ConnectedToBus_4;
            ch5_radiobutton.IsEnabled = ConnectedToBus_5;
            ch6_radiobutton.IsEnabled = ConnectedToBus_6;
            ch7_radiobutton.IsEnabled = ConnectedToBus_7;
            ch8_radiobutton.IsEnabled = ConnectedToBus_8;
            ch9_radiobutton.IsEnabled = ConnectedToBus_9;
            ch10_radiobutton.IsEnabled = ConnectedToBus_10;
            ch11_radiobutton.IsEnabled = ConnectedToBus_11;
            ch12_radiobutton.IsEnabled = ConnectedToBus_12;
            ch13_radiobutton.IsEnabled = ConnectedToBus_13;
            ch14_radiobutton.IsEnabled = ConnectedToBus_14;
            ch15_radiobutton.IsEnabled = ConnectedToBus_15;
            ch16_radiobutton.IsEnabled = ConnectedToBus_16;
        }
    }
}
