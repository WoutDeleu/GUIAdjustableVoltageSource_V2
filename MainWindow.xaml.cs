using System;
using System.IO;
using System.IO.Ports;
using SerialCom = SerialCommunication;
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
    public partial class MainWindow : Window
    {
        delegate void serialCalback(string val);
        SerialPort serialPort;
        public bool ConnectedToGround_1 { get; set; }
        public bool ConnectedToGround_2 { get; set; }
        public bool ConnectedToGround_3 { get; set; }
        public bool ConnectedToGround_4 { get; set; }
        public bool ConnectedToGround_5 { get; set; }
        public bool ConnectedToGround_6 { get; set; }
        public bool ConnectedToGround_7 { get; set; }
        public bool ConnectedToGround_8 { get; set; }
        public bool ConnectedToGround_9 { get; set; }
        public bool ConnectedToGround_10 { get; set; }
        public bool ConnectedToGround_11 { get; set; }
        public bool ConnectedToGround_12 { get; set; }
        public bool ConnectedToGround_13 { get; set; }
        public bool ConnectedToGround_14 { get; set; }
        public bool ConnectedToGround_15 { get; set; }
        public bool ConnectedToGround_16 { get; set; }

        public bool ConnectedToBus_1 { get; set; }
        public bool ConnectedToBus_2 { get; set; }
        public bool ConnectedToBus_3 { get; set; }
        public bool ConnectedToBus_4 { get; set; }
        public bool ConnectedToBus_5 { get; set; }
        public bool ConnectedToBus_6 { get; set; }
        public bool ConnectedToBus_7 { get; set; }
        public bool ConnectedToBus_8 { get; set; }
        public bool ConnectedToBus_9 { get; set; }
        public bool ConnectedToBus_10 { get; set; }
        public bool ConnectedToBus_11 { get; set; }
        public bool ConnectedToBus_12 { get; set; }
        public bool ConnectedToBus_13 { get; set; }
        public bool ConnectedToBus_14 { get; set; }
        public bool ConnectedToBus_15 { get; set; }
        public bool ConnectedToBus_16 { get; set; }


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

        private bool updatedGnd { get; set; }
        private bool updatedBus { get; set; }
        private double voltage { get; set; }


        private void OpenSettingScreen(object sender, RoutedEventArgs e)
        {
            SettingScreen settings = new SettingScreen(serialPort);
            settings.ShowDialog();
        }
        private void OpenMeasureScreen(object sender, RoutedEventArgs e)
        {
            MeasureScreen measureScreen = new MeasureScreen(serialPort);
            measureScreen.ShowDialog();
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
                    serialPort.Open();

                    SerialCom.writeSerialPort((int)SerialCom.Functions.PUT_VOLTAGE + "," + voltage + ";", serialPort);
                    Debug.WriteLine("has sent");

                    serialPort.Close();
                }
                catch (IOException ex)
                {
                    Debug.WriteLine(ex);
                }

                updatedBus = true;
            }
            else Debug.WriteLine("Invalid number");
        }
        private void Connect2Ground(object sender, RoutedEventArgs e)
        {
            if (!updatedGnd)
            {
                string data = (int)SerialCom.Functions.CONNECT_TO_GROUND + ",";
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
                data = data + 16 + "," + ConnectedToGround_16 + ",";
                data = data + ";";
                Debug.WriteLine(data);

                updatedGnd = true;
            }
        }

        private void Connect2Bus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(updatedBus);
            if (!updatedBus)
            {
                string data = (int)SerialCom.Functions.CONNECT_TO_BUS + ",";
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
                Debug.WriteLine(data);

                updatedBus = true;
            }
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            serialPort = new SerialPort("COM5", 115200);
            // serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            updatedGnd = true;
            updatedBus = true;
            // SerialCom.initSerialPort(serialPort);
        }
        public void toggleLed(object sender, RoutedEventArgs e)
        {
            try
            {
                // SerialCom.openSerialPort(serialPort);
                serialPort.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append((int)SerialCom.Functions.TOGGLE_LED);
                sb.Append(";");

                SerialCom.writeSerialPort(sb.ToString(), serialPort);
                readSerialPort();

                serialPort.Close();
                // SerialCom.openSerialPort(serialPort);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

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
    }
}
