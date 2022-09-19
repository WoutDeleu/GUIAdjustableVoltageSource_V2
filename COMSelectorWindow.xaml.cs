using System.Windows;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Media.TextFormatting;
using System;
using System.Diagnostics.Metrics;

namespace AdjustableVoltageSource
{
	public partial class COMSelectorWindow : Window
	{
		readonly MainWindow mw;
		bool applied;

		public COMSelectorWindow(List<string> comports, MainWindow mw)
        {
            this.mw = mw;

			applied = false;

            InitializeComponent();

            counter.Content = comports.Count + " COM ports were automatically found in Win32 Registery.";
            COMSelector.ItemsSource = comports;

            DataContext = this;
		}

		private void ApplyCOMPort(object sender, RoutedEventArgs e)
		{
			if(COMSelector.SelectedItem != null)
			{
				mw.StatusBox_Status = "Selected COM port : " + COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
				mw.ret_port = COMSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
				applied = true;

                Close();
			}
			else
			{
				MessageBox.Show("Select a COM-port", "", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		
		// Configure close-button-action
        protected override void OnClosing(CancelEventArgs e)
		{
			if (applied)
			{
				e.Cancel = false;
			}
			else
			{
				mw.Close();
			}
        }
    }
}