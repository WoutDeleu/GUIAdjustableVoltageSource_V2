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

    public partial class MeasureScreen : Window, INotifyPropertyChanged
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public static Communicator communicator;
        private string _measuredValue;
        private bool[] connected;
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
        
        public bool ch1_connected
        {
            get { return connected[0]; }
        }
        public bool ch2_connected
        {
            get { return connected[1]; }
        }
        public bool ch3_connected
        {
            get { return connected[2]; }
        }
        public bool ch4_connected
        {
            get { return connected[3]; }
        }
        public bool ch5_connected
        {
            get { return connected[4]; }
        }
        public bool ch6_connected
        {
            get { return connected[5]; }
        }
        public bool ch7_connected
        {
            get { return connected[6]; }
        }
        public bool ch8_connected
        {
            get { return connected[7]; }
        }
        public bool ch9_connected
        {
            get { return connected[8]; }
        }
        public bool ch10_connected
        {
            get { return connected[9]; }
        }
        public bool ch11_connected
        {
            get { return connected[10]; }
        } 
        public bool ch12_connected
        {
            get { return connected[11]; }
        }
        public bool ch13_connected
        {
            get { return connected[12]; }
        }
        public bool ch14_connected
        {
            get { return connected[13]; }
        }
        public bool ch15_connected
        {
            get { return connected[14]; }
        }
        public bool ch16_connected
        {
            get { return connected[15]; }
        }
        
        public MeasureScreen(Communicator s, bool[] con)
        {
            SelectionVisible = Visibility.Visible;

            communicator = s;
            connected = con;

            DataContext = this;
            InitializeComponent();
            Current_MeasuredValue.SetBinding(ContentProperty, new Binding("MeasuredValue"));
        }
        private void CloseMeasureScreen(object sender, RoutedEventArgs e)
        {
            Close();
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
                    mw.StatusBox_Error = ("Fault in measure format");
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
                    Debug.WriteLine("Fault in measure format...");
                    MeasuredValue = "FAULT";
                }
            }
        }

        private bool handle = true;
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
            Debug.WriteLine(SelectionVisible);
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
                Debug.WriteLine("FAULT IN COMMUNICATION");
                return "FAULT IN COMMUNICATION";
            }
            else
            {
                Debug.WriteLine(s.Substring(begin + 1, (end - begin - 1)));
                return s.Substring(begin + 1, (end - begin - 1));
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
