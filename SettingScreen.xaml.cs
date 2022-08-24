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

namespace AdjustableVoltageSource
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingScreen : Window, INotifyPropertyChanged
    {
        public static SerialPort serialPort;
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
        public SettingScreen(SerialPort s)
        {    
            InitializeComponent();
            BoardNumber = getBoardNumberArduino();
            Current_BoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
            serialPort = s;
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
				// TODO
            }
            else
            {
                Debug.WriteLine("Fault in fomrat input");
                // ... -> foutmelding
            }
        }
        public void setBoardNumberArduino(int boardNumber)
        {
			// TODO
        }

        private int getBoardNumberArduino()
        {
			// TODO
            int boardNumber;
            
            boardNumber = 0;
            return boardNumber;
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
