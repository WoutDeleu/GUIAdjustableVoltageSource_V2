using System.Windows;
using System.ComponentModel;
using System;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _ConnectedToGround_1;
        private bool _ConnectedToBus_1;
        public bool ConnectedToGround_1
        {
            get
            {
                return _ConnectedToGround_1;
            }
            set
            {
                if (value) ConnectedToBus_1 = false;
                _ConnectedToGround_1 = value;
                OnPropertyChanged("ConnectedToGround_1");
            }
        }
        public bool ConnectedToBus_1
        {
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
        
        private void DisconnectAll()
        {
            ConnectedToBus_1 = false;
            ConnectedToBus_2 = false;
            ConnectedToBus_3 = false;
            ConnectedToBus_4 = false;
            ConnectedToBus_5 = false;
            ConnectedToBus_6 = false;
            ConnectedToBus_7 = false;
            ConnectedToBus_8 = false;
            ConnectedToBus_9 = false;
            ConnectedToBus_10 = false;
            ConnectedToBus_11 = false;
            ConnectedToBus_12 = false;
            ConnectedToBus_13 = false;
            ConnectedToBus_14 = false;
            ConnectedToBus_15 = false;
            ConnectedToBus_16 = false;

            ConnectedToGround_1 = false;
            ConnectedToGround_2 = false;
            ConnectedToGround_3 = false;
            ConnectedToGround_4 = false;
            ConnectedToGround_5 = false;
            ConnectedToGround_6 = false;
            ConnectedToGround_7 = false;
            ConnectedToGround_8 = false;
            ConnectedToGround_9 = false;
            ConnectedToGround_10 = false;
            ConnectedToGround_11 = false;
            ConnectedToGround_12 = false;
            ConnectedToGround_13 = false;
            ConnectedToGround_14 = false;
            ConnectedToGround_15 = false;
            ConnectedToGround_16 = false;

            ch1.IsEnabled = false;
            ch2.IsEnabled = false;
            ch3.IsEnabled = false;
            ch4.IsEnabled = false;
            ch5.IsEnabled = false;
            ch6.IsEnabled = false;
            ch7.IsEnabled = false;
            ch8.IsEnabled = false;
            ch9.IsEnabled = false;
            ch10.IsEnabled = false;
            ch11.IsEnabled = false;
            ch12.IsEnabled = false;
            ch13.IsEnabled = false;
            ch14.IsEnabled = false;
            ch15.IsEnabled = false;
            ch16.IsEnabled = false;

            communicator.writeSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");
            VoltageTextBox.Text = "";
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if (!updatedBus || !updatedGnd)
            {
                string data = FormatGrounddata_pt1();
                communicator.writeSerialPort(data);
                data = FormatGrounddata_pt2();
                communicator.writeSerialPort(data);

                data = FormatBusdata_pt1();
                communicator.writeSerialPort(data);
                data = FormatBusdata_pt2();
                communicator.writeSerialPort(data);

                updateMeasureBoxes();

                updatedBus = true;
                updatedGnd = true;
            }
            else StatusBox_Status = "Nothing to update";
        }
    }
}
