using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

        private bool handle = true;

        private void measureVoltageCmd()
        {
            string channel = "";
            if (ch1.IsChecked == true) channel = "1";
            else if (ch2.IsChecked == true) channel = "2";
            else if (ch3.IsChecked == true) channel = "3";
            else if (ch4.IsChecked == true) channel = "4";
            else if (ch5.IsChecked == true) channel = "5";
            else if (ch6.IsChecked == true) channel = "6";
            else if (ch7.IsChecked == true) channel = "7";
            else if (ch8.IsChecked == true) channel = "8";
            else if (ch9.IsChecked == true) channel = "9";
            else if (ch10.IsChecked == true) channel = "10";
            else if (ch11.IsChecked == true) channel = "11";
            else if (ch12.IsChecked == true) channel = "12";
            else if (ch13.IsChecked == true) channel = "13";
            else if (ch14.IsChecked == true) channel = "14";
            else if (ch15.IsChecked == true) channel = "15";
            else if (ch16.IsChecked == true) channel = "16";

            communicator.writeSerialPort((int)Communicator.Functions.MEASURE_VOLTAGE + "," + channel + ";");
        }
        private void MeasureValue(object sender, RoutedEventArgs e)
        {
            if (SelectMeasureFunction.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Measure Current")
            {
                double current_out;
                communicator.writeSerialPort((int)Communicator.Functions.MEASURE_CURRENT + ";");

                String input ="";

                while(communicator.serialPort.BytesToRead != 0)
                {
                    input += communicator.serialPort.ReadExisting();
                }
                string current = Communicator.ExtractInput(input, this).Replace(".", ",");
                if (double.TryParse(current, out current_out))
                    MeasuredValue = current_out + " A";
                else
                {
                    StatusBox_Error = "Fault in measure format";
                    MeasuredValue = "FAULT";
                }
            }                
            else
            {
                double voltage_out;
                measureVoltageCmd();

                String input = "";

                while (communicator.serialPort.BytesToRead != 0)
                {
                    input += communicator.serialPort.ReadExisting();
                }
                Debug.WriteLine(input);
                string voltage = Communicator.ExtractInput(input, this).Replace(".", ",");
                if (double.TryParse(voltage, out voltage_out))
                    MeasuredValue = voltage_out + " V";
                else
                {
                    StatusBox_Error = "Fault in measure format...";
                    MeasuredValue = "FAULT";
                }
            }
        }
        private void selectMeasureFunction_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }
        private void selectMeasureFunction_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }
        private void Handle()
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
        private void updateMeasureBoxes()
        {
            ch1.IsEnabled = ConnectedToBus_1;
            ch2.IsEnabled = ConnectedToBus_2;
            ch3.IsEnabled = ConnectedToBus_3;
            ch4.IsEnabled = ConnectedToBus_4;
            ch5.IsEnabled = ConnectedToBus_5;
            ch6.IsEnabled = ConnectedToBus_6;
            ch7.IsEnabled = ConnectedToBus_7;
            ch8.IsEnabled = ConnectedToBus_8;
            ch9.IsEnabled = ConnectedToBus_9;
            ch10.IsEnabled = ConnectedToBus_10;
            ch11.IsEnabled = ConnectedToBus_11;
            ch12.IsEnabled = ConnectedToBus_12;
            ch13.IsEnabled = ConnectedToBus_13;
            ch14.IsEnabled = ConnectedToBus_14;
            ch15.IsEnabled = ConnectedToBus_15;
            ch16.IsEnabled = ConnectedToBus_16;
        }
    }
}
