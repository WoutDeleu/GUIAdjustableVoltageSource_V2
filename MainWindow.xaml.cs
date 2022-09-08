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
using System.Threading;

namespace AdjustableVoltageSource
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        delegate void serialCalback(string val);
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        Tierce tierce = new Tierce();


        private int boolToInt(bool boolean)
        {
            if (boolean) return 1;
            else return 0;
        }
        private string formatBusdata_pt1()
        {
            string data = (int)Tierce.Functions.CONNECT_TO_BUS + ",";
            data = data + 1 + "," + boolToInt(ConnectedToBus_1) + ",";
            data = data + 2 + "," + boolToInt(ConnectedToBus_2) + ",";
            data = data + 3 + "," + boolToInt(ConnectedToBus_3) + ",";
            data = data + 4 + "," + boolToInt(ConnectedToBus_4) + ",";
            data = data + 5 + "," + boolToInt(ConnectedToBus_5) + ",";
            data = data + 6 + "," + boolToInt(ConnectedToBus_6) + ",";
            data = data + 7 + "," + boolToInt(ConnectedToBus_7) + ",";
            data = data + 8 + "," + boolToInt(ConnectedToBus_8) + ",";
            data = data + ";";
            return data;
        }
        private string formatBusdata_pt2() 
        {
            string data = (int)Tierce.Functions.CONNECT_TO_BUS + ",";
            data = data + 9 + "," + boolToInt(ConnectedToBus_9) + ",";
            data = data + 10 + "," + boolToInt(ConnectedToBus_10) + ",";
            data = data + 11 + "," + boolToInt(ConnectedToBus_11) + ",";
            data = data + 12 + "," + boolToInt(ConnectedToBus_12) + ",";
            data = data + 13 + "," + boolToInt(ConnectedToBus_13) + ",";
            data = data + 14 + "," + boolToInt(ConnectedToBus_14) + ",";
            data = data + 15 + "," + boolToInt(ConnectedToBus_15) + ",";
            data = data + 16 + "," + boolToInt(ConnectedToBus_16);
            data = data + ";";
            return data;
        }
        private string formatGrounddata_pt1()
        {
            string data = (int)Tierce.Functions.CONNECT_TO_GROUND + ",";
            data = data + 1 + "," + boolToInt(ConnectedToGround_1) + ",";
            data = data + 2 + "," + boolToInt(ConnectedToGround_2) + ",";
            data = data + 3 + "," + boolToInt(ConnectedToGround_3) + ",";
            data = data + 4 + "," + boolToInt(ConnectedToGround_4) + ",";
            data = data + 5 + "," + boolToInt(ConnectedToGround_5) + ",";
            data = data + 6 + "," + boolToInt(ConnectedToGround_6) + ",";
            data = data + 7 + "," + boolToInt(ConnectedToGround_7) + ",";
            data = data + 8 + "," + boolToInt(ConnectedToGround_8) + ",";
            data = data + ";";
            return data;
        }
        private string formatGrounddata_pt2() {
            string data = (int)Tierce.Functions.CONNECT_TO_GROUND + ",";
            data = data + 9 + "," + boolToInt(ConnectedToGround_9) + ",";
            data = data + 10 + "," + boolToInt(ConnectedToGround_10) + ",";
            data = data + 11 + "," + boolToInt(ConnectedToGround_11) + ",";
            data = data + 12 + "," + boolToInt(ConnectedToGround_12) + ",";
            data = data + 13 + "," + boolToInt(ConnectedToGround_13) + ",";
            data = data + 14 + "," + boolToInt(ConnectedToGround_14) + ",";
            data = data + 15 + "," + boolToInt(ConnectedToGround_15) + ",";
            data = data + 16 + "," + boolToInt(ConnectedToGround_16);
            data = data + ";";
            return data;
        }
        private bool[] formatBusConnectArray()
        {
            bool[] connected = new bool[16];
            connected[0] = ConnectedToBus_1;
            connected[1] = ConnectedToBus_2;
            connected[2] = ConnectedToBus_3;
            connected[3] = ConnectedToBus_4;
            connected[4] = ConnectedToBus_5;
            connected[5] = ConnectedToBus_6;
            connected[6] = ConnectedToBus_7;
            connected[7] = ConnectedToBus_8;
            connected[8] = ConnectedToBus_9;
            connected[9] = ConnectedToBus_10;
            connected[10] = ConnectedToBus_11;
            connected[11] = ConnectedToBus_12;
            connected[12] = ConnectedToBus_13;
            connected[13] = ConnectedToBus_14;
            connected[14] = ConnectedToBus_15;
            connected[15] = ConnectedToBus_16;
            return connected;
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


        
        private void PutVoltage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string voltagestr = VoltageTextBox.Text.Replace(".", ",");
            if (isValidVoltage(voltagestr))
            {
                voltage = Convert.ToDouble(voltagestr);
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
        public void DisconnectVoltage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            clearVoltageTextbox();
            tierce.writeSerialPort((int)Tierce.Functions.DISCONNECT_VOLTAGE + ";");
        }
        private void clearVoltageTextbox()
        {
            VoltageTextBox.Text = "";
        }
        private string extractInput(string s)
        {
            char[] array = s.ToCharArray();
            int begin = 0, end = 0;
            bool foundBegin = false;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == '|' && !foundBegin)
                {
                    foundBegin = true;
                    begin = i;
                }
                if (array[i] == '|' && begin !=i) end = i;
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


    }
}
