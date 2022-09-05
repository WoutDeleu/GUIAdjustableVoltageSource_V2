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
        public static Tierce tierce;
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
        public SettingScreen(Tierce s)
        {
            tierce = s;
            InitializeComponent();
            BoardNumber = getBoardNumberArduino();
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
            Debug.WriteLine(boardNumberStr);
            if (isValidBoardNumber(boardNumberStr))
            {
                BoardNumber = Convert.ToInt32(boardNumberStr);
				setBoardNumberArduino(BoardNumber);
                Close();
            }
            else
            {
                Debug.WriteLine("Fault in fomrat input");
            }
        }
        public void setBoardNumberArduino(int boardNumber)
        {
            try 
            {
                tierce.writeSerialPort((int)Tierce.Functions.CHANGE_BOARDNUMBER + "," + boardNumber + ";");
            }
            catch(IOException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        private int getBoardNumberArduino()
        {
            int boardNumber;

            tierce.writeSerialPort((int)Tierce.Functions.GET_BOARDNUMBER + ";");

            string input = "";
            while (tierce.serialPort.BytesToRead != 0)
            {
                input += tierce.serialPort.ReadExisting();
            }
            string nr = extractInput(input);
            if (int.TryParse(nr, out boardNumber)) return boardNumber;
            else
            {
                Debug.WriteLine("Fault in fetching boardNumber...");
                return 999999;
            } 
        }

        private string extractInput(string s)
        {
            char[] array = s.ToCharArray();
            int begin = 0, end = 0, length;
            for (int i = 0; i < array.Length; i++)
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

        private Boolean isValidBoardNumber(String s)
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
