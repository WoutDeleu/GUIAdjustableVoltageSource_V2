
using System.Windows;
using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Windows.Data;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _IsConnectedToGround_1;
        private bool _IsConnectedToBus_1;
        public bool IsConnectedToGround_1
        {
            get
            {
                return _IsConnectedToGround_1;
            }
            set
            {
                if (value) IsConnectedToBus_1 = false;
                _IsConnectedToGround_1 = value;
                OnPropertyChanged("IsConnectedToGround_1");
            }
        }
        public bool IsConnectedToBus_1
        {
            get
            {
                return _IsConnectedToBus_1;
            }
            set
            {
                if (value) IsConnectedToGround_1 = false;
                _IsConnectedToBus_1 = value;
                OnPropertyChanged("IsConnectedToBus_1");
            }
        }


        private bool _IsConnectedToGround_2;
        private bool _IsConnectedToBus_2;
        public bool IsConnectedToGround_2
        {
            get
            {
                return _IsConnectedToGround_2;
            }
            set
            {
                if (value) IsConnectedToBus_2 = false;
                _IsConnectedToGround_2 = value;
                OnPropertyChanged("IsConnectedToGround_2");
            }
        }
        public bool IsConnectedToBus_2
        {
            get
            {
                return _IsConnectedToBus_2;
            }
            set
            {
                if (value) IsConnectedToGround_2 = false;
                _IsConnectedToBus_2 = value;
                OnPropertyChanged("IsConnectedToBus_2");
            }
        }


        private bool _IsConnectedToGround_3;
        private bool _IsConnectedToBus_3;
        public bool IsConnectedToGround_3
        {
            get
            {
                return _IsConnectedToGround_3;
            }
            set
            {
                if (value) IsConnectedToBus_3 = false;
                _IsConnectedToGround_3 = value;
                OnPropertyChanged("IsConnectedToGround_3");
            }
        }
        public bool IsConnectedToBus_3
        {
            get
            {
                return _IsConnectedToBus_3;
            }
            set
            {
                if (value) IsConnectedToGround_3 = false;
                _IsConnectedToBus_3 = value;
                OnPropertyChanged("IsConnectedToBus_3");
            }
        }


        private bool _IsConnectedToGround_4;
        private bool _IsConnectedToBus_4;
        public bool IsConnectedToGround_4
        {
            get
            {
                return _IsConnectedToGround_4;
            }
            set
            {
                if (value) IsConnectedToBus_4 = false;
                _IsConnectedToGround_4 = value;
                OnPropertyChanged("IsConnectedToGround_4");
            }
        }
        public bool IsConnectedToBus_4
        {
            get
            {
                return _IsConnectedToBus_4;
            }
            set
            {
                if (value) IsConnectedToGround_4 = false;
                _IsConnectedToBus_4 = value;
                OnPropertyChanged("IsConnectedToBus_4");
            }
        }


        private bool _IsConnectedToGround_5;
        private bool _IsConnectedToBus_5;
        public bool IsConnectedToGround_5
        {
            get
            {
                return _IsConnectedToGround_5;
            }
            set
            {
                if (value) IsConnectedToBus_5 = false;
                _IsConnectedToGround_5 = value;
                OnPropertyChanged("IsConnectedToGround_5");
            }
        }
        public bool IsConnectedToBus_5
        {
            get
            {
                return _IsConnectedToBus_5;
            }
            set
            {
                if (value) IsConnectedToGround_5 = false;
                _IsConnectedToBus_5 = value;
                OnPropertyChanged("IsConnectedToBus_5");
            }
        }


        private bool _IsConnectedToGround_6;
        private bool _IsConnectedToBus_6;
        public bool IsConnectedToGround_6
        {
            get
            {
                return _IsConnectedToGround_6;
            }
            set
            {
                if (value) IsConnectedToBus_6 = false;
                _IsConnectedToGround_6 = value;
                OnPropertyChanged("IsConnectedToGround_6");
            }
        }
        public bool IsConnectedToBus_6
        {
            get
            {
                return _IsConnectedToBus_6;
            }
            set
            {
                if (value) IsConnectedToGround_6 = false;
                _IsConnectedToBus_6 = value;
                OnPropertyChanged("IsConnectedToBus_6");
            }
        }


        private bool _IsConnectedToGround_7;
        private bool _IsConnectedToBus_7;
        public bool IsConnectedToGround_7
        {
            get
            {
                return _IsConnectedToGround_7;
            }
            set
            {
                if (value) IsConnectedToBus_7 = false;
                _IsConnectedToGround_7 = value;
                OnPropertyChanged("IsConnectedToGround_7");
            }
        }
        public bool IsConnectedToBus_7
        {
            get
            {
                return _IsConnectedToBus_7;
            }
            set
            {
                if (value) IsConnectedToGround_7 = false;
                _IsConnectedToBus_7 = value;
                OnPropertyChanged("IsConnectedToBus_7");
            }
        }


        private bool _IsConnectedToGround_8;
        private bool _IsConnectedToBus_8;
        public bool IsConnectedToGround_8
        {
            get
            {
                return _IsConnectedToGround_8;
            }
            set
            {
                if (value) IsConnectedToBus_8 = false;
                _IsConnectedToGround_8 = value;
                OnPropertyChanged("IsConnectedToGround_8");
            }
        }
        public bool IsConnectedToBus_8
        {
            get
            {
                return _IsConnectedToBus_8;
            }
            set
            {
                if (value) IsConnectedToGround_8 = false;
                _IsConnectedToBus_8 = value;
                OnPropertyChanged("IsConnectedToBus_8");
            }
        }


        private bool _IsConnectedToGround_9;
        private bool _IsConnectedToBus_9;
        public bool IsConnectedToGround_9
        {
            get
            {
                return _IsConnectedToGround_9;
            }
            set
            {
                if (value) IsConnectedToBus_9 = false;
                _IsConnectedToGround_9 = value;
                OnPropertyChanged("IsConnectedToGround_9");
            }
        }
        public bool IsConnectedToBus_9
        {
            get
            {
                return _IsConnectedToBus_9;
            }
            set
            {
                if (value) IsConnectedToGround_9 = false;
                _IsConnectedToBus_9 = value;
                OnPropertyChanged("IsConnectedToBus_9");
            }
        }


        private bool _IsConnectedToGround_10;
        private bool _IsConnectedToBus_10;
        public bool IsConnectedToGround_10
        {
            get
            {
                return _IsConnectedToGround_10;
            }
            set
            {
                if (value) IsConnectedToBus_10 = false;
                _IsConnectedToGround_10 = value;
                OnPropertyChanged("IsConnectedToGround_10");
            }
        }
        public bool IsConnectedToBus_10
        {
            get
            {
                return _IsConnectedToBus_10;
            }
            set
            {
                if (value) IsConnectedToGround_10 = false;
                _IsConnectedToBus_10 = value;
                OnPropertyChanged("IsConnectedToBus_10");
            }
        }


        private bool _IsConnectedToGround_11;
        private bool _IsConnectedToBus_11;
        public bool IsConnectedToGround_11
        {
            get
            {
                return _IsConnectedToGround_11;
            }
            set
            {
                if (value) IsConnectedToBus_11 = false;
                _IsConnectedToGround_11 = value;
                OnPropertyChanged("IsConnectedToGround_11");
            }
        }
        public bool IsConnectedToBus_11
        {
            get
            {
                return _IsConnectedToBus_11;
            }
            set
            {
                if (value) IsConnectedToGround_11 = false;
                _IsConnectedToBus_11 = value;
                OnPropertyChanged("IsConnectedToBus_11");
            }
        }


        private bool _IsConnectedToGround_12;
        private bool _IsConnectedToBus_12;
        public bool IsConnectedToGround_12
        {
            get
            {
                return _IsConnectedToGround_12;
            }
            set
            {
                if (value) IsConnectedToBus_12 = false;
                _IsConnectedToGround_12 = value;
                OnPropertyChanged("IsConnectedToGround_12");
            }
        }
        public bool IsConnectedToBus_12
        {
            get
            {
                return _IsConnectedToBus_12;
            }
            set
            {
                if (value) IsConnectedToGround_12 = false;
                _IsConnectedToBus_12 = value;
                OnPropertyChanged("IsConnectedToBus_12");
            }
        }


        private bool _IsConnectedToGround_13;
        private bool _IsConnectedToBus_13;
        public bool IsConnectedToGround_13
        {
            get
            {
                return _IsConnectedToGround_13;
            }
            set
            {
                if (value) IsConnectedToBus_13 = false;
                _IsConnectedToGround_13 = value;
                OnPropertyChanged("IsConnectedToGround_13");
            }
        }
        public bool IsConnectedToBus_13
        {
            get
            {
                return _IsConnectedToBus_13;
            }
            set
            {
                if (value) IsConnectedToGround_13 = false;
                _IsConnectedToBus_13 = value;
                OnPropertyChanged("IsConnectedToBus_13");
            }
        }
        

        private bool _IsConnectedToGround_14;
        private bool _IsConnectedToBus_14;
        public bool IsConnectedToGround_14
        {
            get
            {
                return _IsConnectedToGround_14;
            }
            set
            {
                if (value) IsConnectedToBus_14 = false;
                _IsConnectedToGround_14 = value;
                OnPropertyChanged("IsConnectedToGround_14");
            }
        }
        public bool IsConnectedToBus_14
        {
            get
            {
                return _IsConnectedToBus_14;
            }
            set
            {
                if (value) IsConnectedToGround_14 = false;
                _IsConnectedToBus_14 = value;
                OnPropertyChanged("IsConnectedToBus_14");
            }
        }


        private bool _IsConnectedToGround_15;
        private bool _IsConnectedToBus_15;
        public bool IsConnectedToGround_15
        {
            get
            {
                return _IsConnectedToGround_15;
            }
            set
            {
                if (value) IsConnectedToBus_15 = false;
                _IsConnectedToGround_15 = value;
                OnPropertyChanged("IsConnectedToGround_15");
            }
        }
        public bool IsConnectedToBus_15
        {
            get
            {
                return _IsConnectedToBus_15;
            }
            set
            {
                if (value) IsConnectedToGround_15 = false;
                _IsConnectedToBus_15 = value;
                OnPropertyChanged("IsConnectedToBus_15");
            }
        }


        private bool _IsConnectedToGround_16;
        private bool _IsConnectedToBus_16;
        public bool IsConnectedToGround_16
        {
            get
            {
                return _IsConnectedToGround_16;
            }
            set
            {
                if (value) IsConnectedToBus_16 = false;
                _IsConnectedToGround_16 = value;
                OnPropertyChanged("IsConnectedToGround_16");
            }
        }
        public bool IsConnectedToBus_16
        {
            get
            {
                return _IsConnectedToBus_16;
            }
            set
            {
                if (value) IsConnectedToGround_16 = false;
                _IsConnectedToBus_16 = value;
                OnPropertyChanged("IsConnectedToBus_16");
            }
        }


        private void ConnectBus1(object sender, RoutedEventArgs e)
        {
            // IsConnectedToBus_1 = !IsConnectedToBus_1;
            IsConnectedToGround_1 = false;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus2(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_2 = false;
            // IsConnectedToBus_2 = !IsConnectedToBus_2;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus3(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_3 = false;
            // IsConnectedToBus_3 = !IsConnectedToBus_3;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus4(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_4 = false;
            // IsConnectedToBus_4 = !IsConnectedToBus_4;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus5(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_5 = false;
            // IsConnectedToBus_5 = !IsConnectedToBus_5;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus6(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_6 = false;
            // IsConnectedToBus_6 = !IsConnectedToBus_6;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus7(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_7 = false;
            // IsConnectedToBus_7 = !IsConnectedToBus_7;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus8(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_8 = false;
            // IsConnectedToBus_8 = !IsConnectedToBus_8;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus9(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_9 = false;
            // IsConnectedToBus_9 = !IsConnectedToBus_9;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus10(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_10 = false;
            // IsConnectedToBus_10 = !IsConnectedToBus_10;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus11(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_11 = false;
            // IsConnectedToBus_11 = !IsConnectedToBus_11;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus12(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_12 = false;
            // IsConnectedToBus_12 = !IsConnectedToBus_12;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus13(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_13 = false;
            // IsConnectedToBus_13 = !IsConnectedToBus_13;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus14(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_14 = false;
            // IsConnectedToBus_14 = !IsConnectedToBus_14;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus15(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_15 = false;
            // IsConnectedToBus_15 = !IsConnectedToBus_15;
            if (IsBusUpdated) IsBusUpdated = false;
        }
        private void ConnectBus16(object sender, RoutedEventArgs e)
        {
            IsConnectedToGround_16 = false;
            // IsConnectedToBus_16 = !IsConnectedToBus_16;
            if (IsBusUpdated) IsBusUpdated = false;
        }


        private void ConnectGnd1(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_1 = false;
            // IsConnectedToGround_1 = !IsConnectedToGround_1;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd2(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_2 = false;
            // IsConnectedToGround_2 = !IsConnectedToGround_2;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd3(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_3 = false;
            // IsConnectedToGround_3 = !IsConnectedToGround_3;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd4(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_4 = false;
            // IsConnectedToGround_4 = !IsConnectedToGround_4;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd5(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_5 = false;
            // IsConnectedToGround_5 = !IsConnectedToGround_5;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd6(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_6 = false;
            // IsConnectedToGround_6 = !IsConnectedToGround_6;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd7(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_7 = false;
            // IsConnectedToGround_7 = !IsConnectedToGround_7;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd8(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_8 = false;
            // IsConnectedToGround_8 = !IsConnectedToGround_8;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd9(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_9 = false;
            // IsConnectedToGround_9 = !IsConnectedToGround_9;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd10(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_10 = false;
            // IsConnectedToGround_10 = !IsConnectedToGround_10;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd11(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_11 = false;
            // IsConnectedToGround_11 = !IsConnectedToGround_11;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd12(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_12 = false;
            // IsConnectedToGround_12 = !IsConnectedToGround_12;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd13(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_13 = false;
            // IsConnectedToGround_13 = !IsConnectedToGround_13;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd14(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_14 = false;
            // IsConnectedToGround_14 = !IsConnectedToGround_14;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd15(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_15 = false;
            // IsConnectedToGround_15 = !IsConnectedToGround_15;
            if (IsGndUpdated) IsGndUpdated = false;
        }
        private void ConnectGnd16(object sender, RoutedEventArgs e)
        {
            IsConnectedToBus_16 = false;
            // IsConnectedToGround_16 = !IsConnectedToGround_16;
            if (IsGndUpdated) IsGndUpdated = false;
        }


        // Disconnect all channels to bus and ground as selected
        private void DisconnectAll()
        {
            IsConnectedToBus_1 = false;
            IsConnectedToBus_2 = false;
            IsConnectedToBus_3 = false;
            IsConnectedToBus_4 = false;
            IsConnectedToBus_5 = false;
            IsConnectedToBus_6 = false;
            IsConnectedToBus_7 = false;
            IsConnectedToBus_8 = false;
            IsConnectedToBus_9 = false;
            IsConnectedToBus_10 = false;
            IsConnectedToBus_11 = false;
            IsConnectedToBus_12 = false;
            IsConnectedToBus_13 = false;
            IsConnectedToBus_14 = false;
            IsConnectedToBus_15 = false;
            IsConnectedToBus_16 = false;

            IsConnectedToGround_1 = false;
            IsConnectedToGround_2 = false;
            IsConnectedToGround_3 = false;
            IsConnectedToGround_4 = false;
            IsConnectedToGround_5 = false;
            IsConnectedToGround_6 = false;
            IsConnectedToGround_7 = false;
            IsConnectedToGround_8 = false;
            IsConnectedToGround_9 = false;
            IsConnectedToGround_10 = false;
            IsConnectedToGround_11 = false;
            IsConnectedToGround_12 = false;
            IsConnectedToGround_13 = false;
            IsConnectedToGround_14 = false;
            IsConnectedToGround_15 = false;
            IsConnectedToGround_16 = false;

            MeasureVoltageCh1.IsEnabled = false;
            MeasureVoltageCh2.IsEnabled = false;
            MeasureVoltageCh3.IsEnabled = false;
            MeasureVoltageCh4.IsEnabled = false;
            MeasureVoltageCh5.IsEnabled = false;
            MeasureVoltageCh6.IsEnabled = false;
            MeasureVoltageCh7.IsEnabled = false;
            MeasureVoltageCh8.IsEnabled = false;
            MeasureVoltageCh9.IsEnabled = false;
            MeasureVoltageCh10.IsEnabled = false;
            MeasureVoltageCh11.IsEnabled = false;
            MeasureVoltageCh12.IsEnabled = false;
            MeasureVoltageCh13.IsEnabled = false;
            MeasureVoltageCh14.IsEnabled = false;
            MeasureVoltageCh15.IsEnabled = false;
            MeasureVoltageCh16.IsEnabled = false;

            MeasureVoltageCh1.IsChecked = false;
            MeasureVoltageCh2.IsChecked = false;
            MeasureVoltageCh3.IsChecked = false;
            MeasureVoltageCh4.IsChecked = false;
            MeasureVoltageCh5.IsChecked = false;
            MeasureVoltageCh6.IsChecked = false;
            MeasureVoltageCh7.IsChecked = false;
            MeasureVoltageCh8.IsChecked = false;
            MeasureVoltageCh9.IsChecked = false;
            MeasureVoltageCh10.IsChecked = false;
            MeasureVoltageCh11.IsChecked = false;
            MeasureVoltageCh12.IsChecked = false;
            MeasureVoltageCh13.IsChecked = false;
            MeasureVoltageCh14.IsChecked = false;
            MeasureVoltageCh15.IsChecked = false;
            MeasureVoltageCh16.IsChecked = false;

            Communicator.WriteSerialPort((int)Communicator.Functions.DISCONNECT_VOLTAGE + ";");
            SetVoltageTextBox.Text = "";
            StatusBox_Status = "Everything is Disconnected";
        }
        // Disconnect all channels to bus and ground as selected
        private void DisconnectAllWithClosedPort()
        {
            IsConnectedToBus_1 = false;
            IsConnectedToBus_2 = false;
            IsConnectedToBus_3 = false;
            IsConnectedToBus_4 = false;
            IsConnectedToBus_5 = false;
            IsConnectedToBus_6 = false;
            IsConnectedToBus_7 = false;
            IsConnectedToBus_8 = false;
            IsConnectedToBus_9 = false;
            IsConnectedToBus_10 = false;
            IsConnectedToBus_11 = false;
            IsConnectedToBus_12 = false;
            IsConnectedToBus_13 = false;
            IsConnectedToBus_14 = false;
            IsConnectedToBus_15 = false;
            IsConnectedToBus_16 = false;

            IsConnectedToGround_1 = false;
            IsConnectedToGround_2 = false;
            IsConnectedToGround_3 = false;
            IsConnectedToGround_4 = false;
            IsConnectedToGround_5 = false;
            IsConnectedToGround_6 = false;
            IsConnectedToGround_7 = false;
            IsConnectedToGround_8 = false;
            IsConnectedToGround_9 = false;
            IsConnectedToGround_10 = false;
            IsConnectedToGround_11 = false;
            IsConnectedToGround_12 = false;
            IsConnectedToGround_13 = false;
            IsConnectedToGround_14 = false;
            IsConnectedToGround_15 = false;
            IsConnectedToGround_16 = false;

            MeasureVoltageCh1.IsEnabled = false;
            MeasureVoltageCh2.IsEnabled = false;
            MeasureVoltageCh3.IsEnabled = false;
            MeasureVoltageCh4.IsEnabled = false;
            MeasureVoltageCh5.IsEnabled = false;
            MeasureVoltageCh6.IsEnabled = false;
            MeasureVoltageCh7.IsEnabled = false;
            MeasureVoltageCh8.IsEnabled = false;
            MeasureVoltageCh9.IsEnabled = false;
            MeasureVoltageCh10.IsEnabled = false;
            MeasureVoltageCh11.IsEnabled = false;
            MeasureVoltageCh12.IsEnabled = false;
            MeasureVoltageCh13.IsEnabled = false;
            MeasureVoltageCh14.IsEnabled = false;
            MeasureVoltageCh15.IsEnabled = false;
            MeasureVoltageCh16.IsEnabled = false;

            MeasureVoltageCh1.IsChecked = false;
            MeasureVoltageCh2.IsChecked = false;
            MeasureVoltageCh3.IsChecked = false;
            MeasureVoltageCh4.IsChecked = false;
            MeasureVoltageCh5.IsChecked = false;
            MeasureVoltageCh6.IsChecked = false;
            MeasureVoltageCh7.IsChecked = false;
            MeasureVoltageCh8.IsChecked = false;
            MeasureVoltageCh9.IsChecked = false;
            MeasureVoltageCh10.IsChecked = false;
            MeasureVoltageCh11.IsChecked = false;
            MeasureVoltageCh12.IsChecked = false;
            MeasureVoltageCh13.IsChecked = false;
            MeasureVoltageCh14.IsChecked = false;
            MeasureVoltageCh15.IsChecked = false;
            MeasureVoltageCh16.IsChecked = false;

            SetVoltageTextBox.Text = "";
            StatusBox_Status = "Everything is Disconnected";
        }
            
        
        // Defintly connect all channels to bus and ground as selected
        private void Connect(object sender, RoutedEventArgs e)
        {
            // if there are no changes being made, there is no need to update the registers
            if (!IsBusUpdated || !IsGndUpdated)
            {
                string data = FormatGrounddata_pt1();
                Communicator.WriteSerialPort(data);
                data = FormatGrounddata_pt2();
                Communicator.WriteSerialPort(data);

                data = FormatBusdata_pt1();
                Communicator.WriteSerialPort(data);
                data = FormatBusdata_pt2();
                Communicator.WriteSerialPort(data);

                UpdateMeasureBoxes();

                StatusBox_Status = "Connections are established";
                IsBusUpdated = true;
                IsGndUpdated = true;
            }
            else StatusBox_Status = "Nothing to update while connecting";
        }
    }
}
