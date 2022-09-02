using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.VisualBasic;


namespace AdjustableVoltageSource
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        delegate void serialCalback(string val);
        public event PropertyChangedEventHandler PropertyChanged;
        Tierce tierce = new Tierce();
        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        private bool _ConnectedToGround_1;
        private bool _ConnectedToBus_1;
        public bool ConnectedToGround_1 {
            get
            {
                return _ConnectedToGround_1;
            }
            set
            {
                if(value) ConnectedToBus_1 = false;
                _ConnectedToGround_1 = value;
                OnPropertyChanged("ConnectedToGround_1");
            }
        }
        public bool ConnectedToBus_1 {
            get
            {
                return _ConnectedToBus_1;
            }
            set
            {
                if (value) ConnectedToGround_1 = false;
                _ConnectedToBus_1 = value;
                OnPropertyChanged("ConnectedToBus_1");
            }
        }

        
        private bool _ConnectedToGround_2;
        private bool _ConnectedToBus_2;
        public bool ConnectedToGround_2
        {
            get
            {
                return _ConnectedToGround_2;
            }
            set
            {
                if (value) ConnectedToBus_2 = false;
                _ConnectedToGround_2 = value;
                OnPropertyChanged("ConnectedToGround_2");
            }
        }
        public bool ConnectedToBus_2
        {
            get
            {
                return _ConnectedToBus_2;
            }
            set
            {
                if (value) ConnectedToGround_2 = false;
                _ConnectedToBus_2 = value;
                OnPropertyChanged("ConnectedToBus_2");
            }
        }
        
        
        private bool _ConnectedToGround_3;
        private bool _ConnectedToBus_3;
        public bool ConnectedToGround_3
        {
            get
            {
                return _ConnectedToGround_3;
            }
            set
            {
                if (value) ConnectedToBus_3 = false;
                _ConnectedToGround_3 = value;
                OnPropertyChanged("ConnectedToGround_3");
            }
        }
        public bool ConnectedToBus_3
        {
            get
            {
                return _ConnectedToBus_3;
            }
            set
            {
                if (value) ConnectedToGround_3 = false;
                _ConnectedToBus_3 = value;
                OnPropertyChanged("ConnectedToBus_3");
            }
        }
        
        
        private bool _ConnectedToGround_4; 
        private bool _ConnectedToBus_4;
        public bool ConnectedToGround_4
        {
            get
            {
                return _ConnectedToGround_4;
            }
            set
            {
                if (value) ConnectedToBus_4 = false;
                _ConnectedToGround_4 = value;
                OnPropertyChanged("ConnectedToGround_4");
            }
        }
        public bool ConnectedToBus_4
        {
            get
            {
                return _ConnectedToBus_4;
            }
            set
            {
                if (value) ConnectedToGround_4 = false;
                _ConnectedToBus_4 = value;
                OnPropertyChanged("ConnectedToBus_4");
            }
        }
        
        
        private bool _ConnectedToGround_5;
        private bool _ConnectedToBus_5;
        public bool ConnectedToGround_5
        {
            get
            {
                return _ConnectedToGround_5;
            }
            set
            {
                if (value) ConnectedToBus_5 = false;
                _ConnectedToGround_5 = value;
                OnPropertyChanged("ConnectedToGround_5");
            }
        }
        public bool ConnectedToBus_5
        {
            get
            {
                return _ConnectedToBus_5;
            }
            set
            {
                if (value) ConnectedToGround_5 = false;
                _ConnectedToBus_5 = value;
                OnPropertyChanged("ConnectedToBus_5");
            }
        }
          

        private bool _ConnectedToGround_6;
        private bool _ConnectedToBus_6;
        public bool ConnectedToGround_6
        {
            get
            {
                return _ConnectedToGround_6;
            }
            set
            {
                if (value) ConnectedToBus_6 = false;
                _ConnectedToGround_6 = value;
                OnPropertyChanged("ConnectedToGround_6");
            }
        }
        public bool ConnectedToBus_6
        {
            get
            {
                return _ConnectedToBus_6;
            }
            set
            {
                if (value) ConnectedToGround_6 = false;
                _ConnectedToBus_6 = value;
                OnPropertyChanged("ConnectedToBus_6");
            }
        }


        private bool _ConnectedToGround_7;
        private bool _ConnectedToBus_7;
        public bool ConnectedToGround_7
        {
            get
            {
                return _ConnectedToGround_7;
            }
            set
            {
                if (value) ConnectedToBus_7 = false;
                _ConnectedToGround_7 = value;
                OnPropertyChanged("ConnectedToGround_7");
            }
        }
        public bool ConnectedToBus_7
        {
            get
            {
                return _ConnectedToBus_7;
            }
            set
            {
                if (value) ConnectedToGround_7 = false;
                _ConnectedToBus_7 = value;
                OnPropertyChanged("ConnectedToBus_7");
            }
        }


        private bool _ConnectedToGround_8;
        private bool _ConnectedToBus_8;
        public bool ConnectedToGround_8
        {
            get
            {
                return _ConnectedToGround_8;
            }
            set
            {
                if (value) ConnectedToBus_8 = false;
                _ConnectedToGround_8 = value;
                OnPropertyChanged("ConnectedToGround_8");
            }
        }
        public bool ConnectedToBus_8
        {
            get
            {
                return _ConnectedToBus_8;
            }
            set
            {
                if (value) ConnectedToGround_8 = false;
                _ConnectedToBus_8 = value;
                OnPropertyChanged("ConnectedToBus_8");
            }
        }
        

        private bool _ConnectedToGround_9;
        private bool _ConnectedToBus_9;
        public bool ConnectedToGround_9
        {
            get
            {
                return _ConnectedToGround_9;
            }
            set
            {
                if (value) ConnectedToBus_9 = false;
                _ConnectedToGround_9 = value;
                OnPropertyChanged("ConnectedToGround_9");
            }
        }
        public bool ConnectedToBus_9
        {
            get
            {
                return _ConnectedToBus_9;
            }
            set
            {
                if (value) ConnectedToGround_9 = false;
                _ConnectedToBus_9 = value;
                OnPropertyChanged("ConnectedToBus_9");
            }
        }
        

        private bool _ConnectedToGround_10;
        private bool _ConnectedToBus_10;
        public bool ConnectedToGround_10
        {
            get
            {
                return _ConnectedToGround_10;
            }
            set
            {
                if (value) ConnectedToBus_10 = false;
                _ConnectedToGround_10 = value;
                OnPropertyChanged("ConnectedToGround_10");
            }
        }
        public bool ConnectedToBus_10
        {
            get
            {
                return _ConnectedToBus_10;
            }
            set
            {
                if (value) ConnectedToGround_10 = false;
                _ConnectedToBus_10 = value;
                OnPropertyChanged("ConnectedToBus_10");
            }
        }
        

        private bool _ConnectedToGround_11;
        private bool _ConnectedToBus_11;
        public bool ConnectedToGround_11
        {
            get
            {
                return _ConnectedToGround_11;
            }
            set
            {
                if (value) ConnectedToBus_11 = false;
                _ConnectedToGround_11 = value;
                OnPropertyChanged("ConnectedToGround_11");
            }
        }
        public bool ConnectedToBus_11
        {
            get
            {
                return _ConnectedToBus_11;
            }
            set
            {
                if (value) ConnectedToGround_11 = false;
                _ConnectedToBus_11 = value;
                OnPropertyChanged("ConnectedToBus_11");
            }
        }
        

        private bool _ConnectedToGround_12;
        private bool _ConnectedToBus_12;
        public bool ConnectedToGround_12
        {
            get
            {
                return _ConnectedToGround_12;
            }
            set
            {
                if (value) ConnectedToBus_12 = false;
                _ConnectedToGround_12 = value;
                OnPropertyChanged("ConnectedToGround_12");
            }
        }
        public bool ConnectedToBus_12
        {
            get
            {
                return _ConnectedToBus_12;
            }
            set
            {
                if (value) ConnectedToGround_12 = false;
                _ConnectedToBus_12 = value;
                OnPropertyChanged("ConnectedToBus_12");
            }
        }

        
        private bool _ConnectedToGround_13;
        private bool _ConnectedToBus_13;
        public bool ConnectedToGround_13
        {
            get
            {
                return _ConnectedToGround_13;
            }
            set
            {
                if (value) ConnectedToBus_13 = false;
                _ConnectedToGround_13 = value;
                OnPropertyChanged("ConnectedToGround_13");
            }
        }
        public bool ConnectedToBus_13
        {
            get
            {
                return _ConnectedToBus_13;
            }
            set
            {
                if (value) ConnectedToGround_13 = false;
                _ConnectedToBus_13 = value;
                OnPropertyChanged("ConnectedToBus_13");
            }
        }

        
        private bool _ConnectedToGround_14;
        private bool _ConnectedToBus_14;
        public bool ConnectedToGround_14
        {
            get
            {
                return _ConnectedToGround_14;
            }
            set
            {
                if (value) ConnectedToBus_14 = false;
                _ConnectedToGround_14 = value;
                OnPropertyChanged("ConnectedToGround_14");
            }
        }
        public bool ConnectedToBus_14
        {
            get
            {
                return _ConnectedToBus_14;
            }
            set
            {
                if (value) ConnectedToGround_14 = false;
                _ConnectedToBus_14 = value;
                OnPropertyChanged("ConnectedToBus_14");
            }
        }
        
        
        private bool _ConnectedToGround_15;
        private bool _ConnectedToBus_15;
        public bool ConnectedToGround_15
        {
            get
            {
                return _ConnectedToGround_15;
            }
            set
            {
                if (value) ConnectedToBus_15 = false;
                _ConnectedToGround_15 = value;
                OnPropertyChanged("ConnectedToGround_15");
            }
        }
        public bool ConnectedToBus_15
        {
            get
            {
                return _ConnectedToBus_15;
            }
            set
            {
                if (value) ConnectedToGround_15 = false;
                _ConnectedToBus_15 = value;
                OnPropertyChanged("ConnectedToBus_15");
            }
        }


        private bool _ConnectedToGround_16;
        private bool _ConnectedToBus_16;
        public bool ConnectedToGround_16
        {
            get
            {
                return _ConnectedToGround_16;
            }
            set
            {
                if (value) ConnectedToBus_16 = false;
                _ConnectedToGround_16 = value;
                OnPropertyChanged("ConnectedToGround_16");
            }
        }
        public bool ConnectedToBus_16
        {
            get
            {
                return _ConnectedToBus_16;
            }
            set
            {
                if (value) ConnectedToGround_16 = false;
                _ConnectedToBus_16 = value;
                OnPropertyChanged("ConnectedToBus_16");
            }
        }


        private void ConnectBus1(object sender, RoutedEventArgs e)
        {
            // ConnectedToBus_1 = !ConnectedToBus_1;
            ConnectedToGround_1 = false;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus2(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_2 = false;
            // ConnectedToBus_2 = !ConnectedToBus_2;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus3(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_3 = false;
            // ConnectedToBus_3 = !ConnectedToBus_3;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus4(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_4 = false;
            // ConnectedToBus_4 = !ConnectedToBus_4;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus5(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_5 = false;
            // ConnectedToBus_5 = !ConnectedToBus_5;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus6(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_6 = false;
            // ConnectedToBus_6 = !ConnectedToBus_6;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus7(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_7 = false;
            // ConnectedToBus_7 = !ConnectedToBus_7;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus8(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_8 = false;
            // ConnectedToBus_8 = !ConnectedToBus_8;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus9(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_9 = false;
            // ConnectedToBus_9 = !ConnectedToBus_9;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus10(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_10 = false;
            // ConnectedToBus_10 = !ConnectedToBus_10;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus11(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_11 = false;
            // ConnectedToBus_11 = !ConnectedToBus_11;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus12(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_12 = false;
            // ConnectedToBus_12 = !ConnectedToBus_12;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus13(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_13 = false;
            // ConnectedToBus_13 = !ConnectedToBus_13;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus14(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_14 = false;
            // ConnectedToBus_14 = !ConnectedToBus_14;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus15(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_15 = false;
            // ConnectedToBus_15 = !ConnectedToBus_15;
            if (updatedBus) updatedBus = false;
        }
        private void ConnectBus16(object sender, RoutedEventArgs e)
        {
            ConnectedToGround_16 = false;
            // ConnectedToBus_16 = !ConnectedToBus_16;
            if (updatedBus) updatedBus = false;
        }


        private void ConnectGnd1(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_1 = false;
            // ConnectedToGround_1 = !ConnectedToGround_1;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd2(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_2 = false;
            // ConnectedToGround_2 = !ConnectedToGround_2;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd3(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_3 = false;
            // ConnectedToGround_3 = !ConnectedToGround_3;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd4(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_4 = false;
            // ConnectedToGround_4 = !ConnectedToGround_4;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd5(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_5 = false;
            // ConnectedToGround_5 = !ConnectedToGround_5;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd6(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_6 = false;
            // ConnectedToGround_6 = !ConnectedToGround_6;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd7(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_7 = false;
            // ConnectedToGround_7 = !ConnectedToGround_7;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd8(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_8 = false;
            // ConnectedToGround_8 = !ConnectedToGround_8;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd9(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_9 = false;
            // ConnectedToGround_9 = !ConnectedToGround_9;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd10(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_10 = false;
            // ConnectedToGround_10 = !ConnectedToGround_10;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd11(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_11 = false;
            // ConnectedToGround_11 = !ConnectedToGround_11;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd12(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_12 = false;
            // ConnectedToGround_12 = !ConnectedToGround_12;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd13(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_13 = false;
            // ConnectedToGround_13 = !ConnectedToGround_13;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd14(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_14 = false;
            // ConnectedToGround_14 = !ConnectedToGround_14;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd15(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_15 = false;
            // ConnectedToGround_15 = !ConnectedToGround_15;
            if (updatedGnd) updatedGnd = false;
        }
        private void ConnectGnd16(object sender, RoutedEventArgs e)
        {
            ConnectedToBus_16 = false;
            // ConnectedToGround_16 = !ConnectedToGround_16;
            if (updatedGnd) updatedGnd = false;
        }

        private string formatBusdata()
        {
            string data = (int)Tierce.Functions.CONNECT_TO_BUS + ",";
            data = data + 1 + "," + ConnectedToBus_1 + ",";
            data = data + 2 + "," + ConnectedToBus_2 + ",";
            data = data + 3 + "," + ConnectedToBus_3 + ",";
            data = data + 4 + "," + ConnectedToBus_4 + ",";
            data = data + 5 + "," + ConnectedToBus_5 + ",";
            data = data + 6 + "," + ConnectedToBus_6 + ",";
            data = data + 7 + "," + ConnectedToBus_7 + ",";
            data = data + 8 + "," + ConnectedToBus_8 + ",";
            data = data + 9 + "," + ConnectedToBus_9 + ",";
            data = data + 10 + "," + ConnectedToBus_10 + ",";
            data = data + 11 + "," + ConnectedToBus_11 + ",";
            data = data + 12 + "," + ConnectedToBus_12 + ",";
            data = data + 13 + "," + ConnectedToBus_13 + ",";
            data = data + 14 + "," + ConnectedToBus_14 + ",";
            data = data + 15 + "," + ConnectedToBus_15 + ",";
            data = data + 16 + "," + ConnectedToBus_16;
            data = data + ";";
            return data;
        }
        private string formatGrounddata()
        {
            string data = (int)Tierce.Functions.CONNECT_TO_GROUND + ",";
            data = data + 1 + "," + ConnectedToGround_1 + ",";
            data = data + 2 + "," + ConnectedToGround_2 + ",";
            data = data + 3 + "," + ConnectedToGround_3 + ",";
            data = data + 4 + "," + ConnectedToGround_4 + ",";
            data = data + 5 + "," + ConnectedToGround_5 + ",";
            data = data + 6 + "," + ConnectedToGround_6 + ",";
            data = data + 7 + "," + ConnectedToGround_7 + ",";
            data = data + 8 + "," + ConnectedToGround_8 + ",";
            data = data + 9 + "," + ConnectedToGround_9 + ",";
            data = data + 10 + "," + ConnectedToGround_10 + ",";
            data = data + 11 + "," + ConnectedToGround_11 + ",";
            data = data + 12 + "," + ConnectedToGround_12 + ",";
            data = data + 13 + "," + ConnectedToGround_13 + ",";
            data = data + 14 + "," + ConnectedToGround_14 + ",";
            data = data + 15 + "," + ConnectedToGround_15 + ",";
            data = data + 16 + "," + ConnectedToGround_16;
            data = data + ";";
            return data;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isValidVoltage(e.Text.Replace(".", ","));
        }
        private bool isValidVoltage(string s)
        {
            double voltage;
            bool isNumeric = double.TryParse(s, out voltage);
            if (isNumeric)
            {
                if (voltage > 0.0 && voltage < 30.0)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }

        private bool updatedGnd { get; set; }
        private bool updatedBus { get; set; }
        private double voltage { get; set; }


        private void OpenSettingScreen(object sender, RoutedEventArgs e)
        {
            SettingScreen settings = new SettingScreen(tierce);
            settings.ShowDialog();
        }
        private void OpenMeasureScreen(object sender, RoutedEventArgs e)
        {
            MeasureScreen measureScreen = new MeasureScreen(tierce);
            measureScreen.ShowDialog();
        }
        public void CloseMainWindow()
        {
            tierce.closeSerialPort();
            Close();
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            tierce.initSerialPort();
            updatedGnd = true;
            updatedBus = true;
        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            if (!updatedBus)
            {
                string data = formatBusdata();
                Debug.WriteLine("Bus: " + data);
                // Seriële communicatie (volgorde van connecten?)

                data = formatGrounddata();
                Debug.WriteLine("Ground: " + data);
                // Seriële communicatie (volgorde van connecten?)


                updatedBus = true;
            }
        }
        private void PutVoltage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string voltagestr = VoltageTextBox.Text.Replace(".", ",");
            if (isValidVoltage(voltagestr))
            {
                voltage = Convert.ToDouble(voltagestr);

                // TODO
                try
                {
                    tierce.writeSerialPort((int)Tierce.Functions.PUT_VOLTAGE + "," + voltage + ";");
                    // tierce.closeSerialPort();
                }
                catch (IOException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else Debug.WriteLine("Invalid number");
        }
        public void toggleLed(object sender, RoutedEventArgs e)
        {
            try
            {
                tierce.writeSerialPort((int)Tierce.Functions.TOGGLE_LED + ";");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        /*
        void readSerialPort()
        {
            // byte b = (byte)serialPort.ReadByte();
            // char c = (char)serialPort.ReadChar();
            // string line = serialPort.ReadLine();
            string all = serialPort.ReadExisting();

            Debug.WriteLine("--------------------------------------");
            // Debug.WriteLine(b);
            // Debug.WriteLine(c);
            Debug.WriteLine(all);
            // Debug.WriteLine(all);
            Debug.WriteLine("--------------------------------------");
        }
        */
    }
}
