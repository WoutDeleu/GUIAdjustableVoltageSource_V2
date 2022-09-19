
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
	public partial class MainWindow
	{
		private string FormatBusdata_pt1()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_BUS + ",";
			data = data + 1 + "," + BoolToInt(IsConnectedToBus_1) + ",";
			data = data + 2 + "," + BoolToInt(IsConnectedToBus_2) + ",";
			data = data + 3 + "," + BoolToInt(IsConnectedToBus_3) + ",";
			data = data + 4 + "," + BoolToInt(IsConnectedToBus_4) + ",";
			data = data + 5 + "," + BoolToInt(IsConnectedToBus_5) + ",";
			data = data + 6 + "," + BoolToInt(IsConnectedToBus_6) + ",";
			data = data + 7 + "," + BoolToInt(IsConnectedToBus_7) + ",";
			data = data + 8 + "," + BoolToInt(IsConnectedToBus_8) + ",";
			data = data + ";";
			return data;
		}
		private string FormatBusdata_pt2()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_BUS + ",";
			data = data + 9 + "," + BoolToInt(IsConnectedToBus_9) + ",";
			data = data + 10 + "," + BoolToInt(IsConnectedToBus_10) + ",";
			data = data + 11 + "," + BoolToInt(IsConnectedToBus_11) + ",";
			data = data + 12 + "," + BoolToInt(IsConnectedToBus_12) + ",";
			data = data + 13 + "," + BoolToInt(IsConnectedToBus_13) + ",";
			data = data + 14 + "," + BoolToInt(IsConnectedToBus_14) + ",";
			data = data + 15 + "," + BoolToInt(IsConnectedToBus_15) + ",";
			data = data + 16 + "," + BoolToInt(IsConnectedToBus_16);
			data = data + ";";
			return data;
		}
		private string FormatGrounddata_pt1()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_GROUND + ",";
			data = data + 1 + "," + BoolToInt(IsConnectedToGround_1) + ",";
			data = data + 2 + "," + BoolToInt(IsConnectedToGround_2) + ",";
			data = data + 3 + "," + BoolToInt(IsConnectedToGround_3) + ",";
			data = data + 4 + "," + BoolToInt(IsConnectedToGround_4) + ",";
			data = data + 5 + "," + BoolToInt(IsConnectedToGround_5) + ",";
			data = data + 6 + "," + BoolToInt(IsConnectedToGround_6) + ",";
			data = data + 7 + "," + BoolToInt(IsConnectedToGround_7) + ",";
			data = data + 8 + "," + BoolToInt(IsConnectedToGround_8) + ",";
			data = data + ";";
			return data;
		}
		private string FormatGrounddata_pt2()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_GROUND + ",";
			data = data + 9 + "," + BoolToInt(IsConnectedToGround_9) + ",";
			data = data + 10 + "," + BoolToInt(IsConnectedToGround_10) + ",";
			data = data + 11 + "," + BoolToInt(IsConnectedToGround_11) + ",";
			data = data + 12 + "," + BoolToInt(IsConnectedToGround_12) + ",";
			data = data + 13 + "," + BoolToInt(IsConnectedToGround_13) + ",";
			data = data + 14 + "," + BoolToInt(IsConnectedToGround_14) + ",";
			data = data + 15 + "," + BoolToInt(IsConnectedToGround_15) + ",";
			data = data + 16 + "," + BoolToInt(IsConnectedToGround_16);
			data = data + ";";
			return data;
		}

		private static int BoolToInt(bool boolean)
		{
			if (boolean) return 1;
			else return 0;
		}
		private static bool IsValidVoltage(string s)
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
        private bool IsValidCOMPort(string s)
        {
			return Regex.IsMatch(s, "^COM[0-9][0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?$");

        }

        public static SolidColorBrush BrushFromHex(string hexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColorString));
        }

    }
}
