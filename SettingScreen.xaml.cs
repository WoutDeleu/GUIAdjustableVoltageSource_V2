using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        private void ApplyBoardNumber(object sender, RoutedEventArgs e)
        {
            GetBoardNumberArduino();
            //e.Handled = true;
            //String boardNumberStr = NewBoardNumber.Text;
            //if (IsValidBoardNumber(boardNumberStr))
            //{
            //    BoardNumber = Convert.ToInt32(boardNumberStr);
            //    SetBoardNumberArduino(BoardNumber);
            //}
            //else
            //{
            //    StatusBox_Error = "Fault in format boardNumber";
            //}
        }
        public void SetBoardNumberArduino(int boardNumber)
        {
            communicator.writeSerialPort((int)Communicator.Functions.CHANGE_BOARDNUMBER + "," + boardNumber + ";");
        }
        private void GetBoardNumberArduino()
        {
            int boardNumber;

            communicator.writeSerialPort((int)Communicator.Functions.GET_BOARDNUMBER + ";");

            string input = "";
            while (communicator.serialPort.BytesToRead != 0)
            {
                input += communicator.serialPort.ReadExisting();
            }
            Debug.WriteLine(input);
            string nr = Communicator.ExtractInput(input, this);
            Debug.WriteLine("nr: "  + nr);
            if (int.TryParse(nr, out boardNumber)) BoardNumber =  boardNumber;
            else
            {
                StatusBox_Error = "Fault in fetching boardNumber...";
                BoardNumber =  999999;
            } 
        }
        private static bool IsValidBoardNumber(String s)
        {
            return Regex.IsMatch(s, @"^\d+$");
        }
    }
}
