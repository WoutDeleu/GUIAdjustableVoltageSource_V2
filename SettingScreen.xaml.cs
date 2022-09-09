using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Management;
using System.IO;

namespace AdjustableVoltageSource
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingScreen : Window, INotifyPropertyChanged
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public static Communicator communicator;
        private int _boardNumber;
        public int BoardNumber
        {
            get { return _boardNumber; }
            set
            {
                if(value != _boardNumber)
                {
                    _boardNumber = value;
                    OnPropertyChanged("BoardNumber");
                }
            }
        }
        public SettingScreen(Communicator s)
        {
            communicator = s;
            InitializeComponent();
            BoardNumber = GetBoardNumberArduino();
            Current_BoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
            DataContext = this;
        }
        private void CancelBoardNumber(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ApplyBoardNumber(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            String boardNumberStr = NewBoardNumber.Text;
            if (IsValidBoardNumber(boardNumberStr))
            {
                BoardNumber = Convert.ToInt32(boardNumberStr);
                SetBoardNumberArduino(BoardNumber);
            }
            else
            {
                mw.StatusBox_Error = ("Fault in format boardNumber");
            }
        }
        public void SetBoardNumberArduino(int boardNumber)
        {
            communicator.writeSerialPort((int)Communicator.Functions.CHANGE_BOARDNUMBER + "," + boardNumber + ";");
        }
        private int GetBoardNumberArduino()
        {
            int boardNumber;

            communicator.writeSerialPort((int)Communicator.Functions.GET_BOARDNUMBER + ";");

            string input = "";
            while (communicator.serialPort.BytesToRead != 0)
            {
                input += communicator.serialPort.ReadExisting();
            }
            string nr = Communicator.ExtractInput(input);
            if (int.TryParse(nr, out boardNumber)) return boardNumber;
            else
            {
                Debug.WriteLine("Fault in fetching boardNumber...");
                return 999999;
            } 
        }
        private static bool IsValidBoardNumber(String s)
        {
            return Regex.IsMatch(s, @"^\d+$");
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
