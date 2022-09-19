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

        private void SelectMeasureFunction_DropDownClosed(object sender, EventArgs e)
        {
            if (handledMeasureSelection) HandleSelectMeasure();
            handledMeasureSelection = true;
        }
        private void SelectMeasureFunction_Changed(object sender, SelectionChangedEventArgs e)
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
            MeasureVoltageCh1.IsEnabled = IsConnectedToBus_1;
            MeasureVoltageCh2.IsEnabled = IsConnectedToBus_2;
            MeasureVoltageCh3.IsEnabled = IsConnectedToBus_3;
            MeasureVoltageCh4.IsEnabled = IsConnectedToBus_4;
            MeasureVoltageCh5.IsEnabled = IsConnectedToBus_5;
            MeasureVoltageCh6.IsEnabled = IsConnectedToBus_6;
            MeasureVoltageCh7.IsEnabled = IsConnectedToBus_7;
            MeasureVoltageCh8.IsEnabled = IsConnectedToBus_8;
            MeasureVoltageCh9.IsEnabled = IsConnectedToBus_9;
            MeasureVoltageCh10.IsEnabled = IsConnectedToBus_10;
            MeasureVoltageCh11.IsEnabled = IsConnectedToBus_11;
            MeasureVoltageCh12.IsEnabled = IsConnectedToBus_12;
            MeasureVoltageCh13.IsEnabled = IsConnectedToBus_13;
            MeasureVoltageCh14.IsEnabled = IsConnectedToBus_14;
            MeasureVoltageCh15.IsEnabled = IsConnectedToBus_15;
            MeasureVoltageCh16.IsEnabled = IsConnectedToBus_16;
        }
    }
}
