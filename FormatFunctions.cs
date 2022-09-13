
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System;

namespace AdjustableVoltageSource
{
	public partial class MainWindow
	{
		private string FormatBusdata_pt1()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_BUS + ",";
			data = data + 1 + "," + BoolToInt(ConnectedToBus_1) + ",";
			data = data + 2 + "," + BoolToInt(ConnectedToBus_2) + ",";
			data = data + 3 + "," + BoolToInt(ConnectedToBus_3) + ",";
			data = data + 4 + "," + BoolToInt(ConnectedToBus_4) + ",";
			data = data + 5 + "," + BoolToInt(ConnectedToBus_5) + ",";
			data = data + 6 + "," + BoolToInt(ConnectedToBus_6) + ",";
			data = data + 7 + "," + BoolToInt(ConnectedToBus_7) + ",";
			data = data + 8 + "," + BoolToInt(ConnectedToBus_8) + ",";
			data = data + ";";
			return data;
		}
		private string FormatBusdata_pt2()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_BUS + ",";
			data = data + 9 + "," + BoolToInt(ConnectedToBus_9) + ",";
			data = data + 10 + "," + BoolToInt(ConnectedToBus_10) + ",";
			data = data + 11 + "," + BoolToInt(ConnectedToBus_11) + ",";
			data = data + 12 + "," + BoolToInt(ConnectedToBus_12) + ",";
			data = data + 13 + "," + BoolToInt(ConnectedToBus_13) + ",";
			data = data + 14 + "," + BoolToInt(ConnectedToBus_14) + ",";
			data = data + 15 + "," + BoolToInt(ConnectedToBus_15) + ",";
			data = data + 16 + "," + BoolToInt(ConnectedToBus_16);
			data = data + ";";
			return data;
		}
		private string FormatGrounddata_pt1()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_GROUND + ",";
			data = data + 1 + "," + BoolToInt(ConnectedToGround_1) + ",";
			data = data + 2 + "," + BoolToInt(ConnectedToGround_2) + ",";
			data = data + 3 + "," + BoolToInt(ConnectedToGround_3) + ",";
			data = data + 4 + "," + BoolToInt(ConnectedToGround_4) + ",";
			data = data + 5 + "," + BoolToInt(ConnectedToGround_5) + ",";
			data = data + 6 + "," + BoolToInt(ConnectedToGround_6) + ",";
			data = data + 7 + "," + BoolToInt(ConnectedToGround_7) + ",";
			data = data + 8 + "," + BoolToInt(ConnectedToGround_8) + ",";
			data = data + ";";
			return data;
		}
		private string FormatGrounddata_pt2()
		{
			string data = (int)Communicator.Functions.CONNECT_TO_GROUND + ",";
			data = data + 9 + "," + BoolToInt(ConnectedToGround_9) + ",";
			data = data + 10 + "," + BoolToInt(ConnectedToGround_10) + ",";
			data = data + 11 + "," + BoolToInt(ConnectedToGround_11) + ",";
			data = data + 12 + "," + BoolToInt(ConnectedToGround_12) + ",";
			data = data + 13 + "," + BoolToInt(ConnectedToGround_13) + ",";
			data = data + 14 + "," + BoolToInt(ConnectedToGround_14) + ",";
			data = data + 15 + "," + BoolToInt(ConnectedToGround_15) + ",";
			data = data + 16 + "," + BoolToInt(ConnectedToGround_16);
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

	}
}
