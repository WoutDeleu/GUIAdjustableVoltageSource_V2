﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows;

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
            e.Handled = true;
            String boardNumberStr = NewBoardNumber.Text;
            if (IsValidBoardNumber(boardNumberStr))
            {
                BoardNumber = Convert.ToInt32(boardNumberStr);
                SetBoardNumberArduino(BoardNumber);
            }
            else
            {
                StatusBox_Error = "Fault in format boardNumber";
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
            string nr = Communicator.ExtractInput(input, this);
            if (int.TryParse(nr, out boardNumber)) return boardNumber;
            else
            {
                StatusBox_Error = "Fault in fetching boardNumber...";
                return 999999;
            } 
        }
        private static bool IsValidBoardNumber(String s)
        {
            return Regex.IsMatch(s, @"^\d+$");
        }
    }
}
