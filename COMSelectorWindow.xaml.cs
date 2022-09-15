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
		MainWindow mw;
		Communicator cm;
		bool applied;

		public COMSelectorWindow(List<string> comports, MainWindow mw, Communicator cm)
        {
            this.cm = cm;
            this.mw = mw;

			applied = false;

            InitializeComponent();

            //WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //Owner = Application.Current.MainWindow;

            counter.Content = comports.Count + " COM ports were automatically found in Win32 Registery.";
            ComSelector.ItemsSource = comports;


            DataContext = this;
		}

		private void Apply(object sender, RoutedEventArgs e)
		{
			if(ComSelector.SelectedItem != null)
			{
				mw.StatusBox_Status = ("Selected COM port : " + ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
				cm.ret_port = ComSelector.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
				applied = true;

                Close();
			}
			else
			{
				MessageBox.Show("Select a COM-port", "", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		// Disable close-button
        protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			if (applied)
			{
				e.Cancel = false;
			}
		}

    }
}