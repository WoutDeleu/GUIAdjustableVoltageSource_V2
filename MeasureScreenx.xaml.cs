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

        public bool ch1_definitiveConnected
        {
            get { return ConnectedToBus_1 & updatedBus & updatedGnd; }
        }
        public bool ch2_definitiveConnected
        {
            get { return ConnectedToBus_2 & updatedBus & updatedGnd; }
        }
        public bool ch3_definitiveConnected
        {
            get { return ConnectedToBus_3 & updatedBus & updatedGnd; }
        }
        public bool ch4_definitiveConnected
        {
            get { return ConnectedToBus_4 & updatedBus & updatedGnd; }
        }
        public bool ch5_definitiveConnected
        {
            get { return ConnectedToBus_5 & updatedBus & updatedGnd; }
        }
        public bool ch6_definitiveConnected
        {
            get { return ConnectedToBus_6 & updatedBus & updatedGnd; }
        }
        public bool ch7_definitiveConnected
        {
            get { return ConnectedToBus_7 & updatedBus & updatedGnd; }
        }
        public bool ch8_definitiveConnected
        {
            get { return ConnectedToBus_8 & updatedBus & updatedGnd; }
        }
        public bool ch9_definitiveConnected
        {
            get { return ConnectedToBus_9 & updatedBus & updatedGnd; }
        }
        public bool ch10_definitiveConnected
        {
            get { return ConnectedToBus_10 & updatedBus & updatedGnd; }
        }
        public bool ch11_definitiveConnected
        {
            get { return ConnectedToBus_11 & updatedBus & updatedGnd; }
        }
        public bool ch12_definitiveConnected
        {
            get { return ConnectedToBus_12 & updatedBus & updatedGnd; }
        }
        public bool ch13_definitiveConnected
        {
            get { return ConnectedToBus_13 & updatedBus & updatedGnd; }
        }
        public bool ch14_definitiveConnected
        {
            get { return ConnectedToBus_14 & updatedBus & updatedGnd; }
        }
        public bool ch15_definitiveConnected
        {
            get { return ConnectedToBus_15 & updatedBus & updatedGnd; }
        }
        public bool ch16_definitiveConnected
        {
            get { return ConnectedToBus_16 & updatedBus & updatedGnd; }
        }

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
                string current = extractInput(input).Replace(".", ",");
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
                string voltage = extractInput(input).Replace(".", ",");
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
        private string extractInput(string s)
        {
            char[] array = s.ToCharArray();
            int begin = 0, end = 0;
            for(int i=0; i<array.Length; i++)
            {
                if (array[i] == '[') begin = i;
                if (array[i] == ']') end = i;
            }
            if (end == 0)
                {
                StatusBox_Error = "FAULT IN COMMUNICATION";
                return "FAULT IN COMMUNICATION";
            }
            else
            {
                Debug.WriteLine(s.Substring(begin + 1, (end - begin - 1)));
                return s.Substring(begin + 1, (end - begin - 1));
            }
        }

    }
}
