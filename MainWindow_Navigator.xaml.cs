using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            communicator.initSerialPort();
            updatedGnd = true;
            updatedBus = true;

            CommandInterface.SelectAll();
            CommandInterface.Selection.Text = "";

            Status.SelectAll();
            Status.Selection.Text = "";

            Registers.SelectAll();
            Registers.Selection.Text = "";

        }
        private void OpenSettingScreen(object sender, RoutedEventArgs e)
        {

            if (updatedBus && updatedGnd)
            {
                SettingScreen settings = new SettingScreen(communicator);
                settings.ShowDialog();
            }
            else
            {
                Debug.WriteLine("Make sure every connection is updated to board");
            }
        }
        private void OpenMeasureScreen(object sender, RoutedEventArgs e)
        {
            if (updatedBus && updatedGnd)
            {
                MeasureScreen measureScreen = new MeasureScreen(communicator, FormatBusConnectArray());
                measureScreen.ShowDialog();
            }
            else
            {
                Debug.WriteLine("Make sure every connection is updated to board");
            }
        }
        public void CloseMainWindow()
        {
            communicator.closeSerialPort();
            Close();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if (!updatedBus || !updatedGnd)
            {
                string data = FormatGrounddata_pt1();
                communicator.writeSerialPort(data);
                data = FormatGrounddata_pt2();
                communicator.writeSerialPort(data);

                data = FormatBusdata_pt1();
                communicator.writeSerialPort(data);
                data = FormatBusdata_pt2();
                communicator.writeSerialPort(data);

                Debug.WriteLine("");

                updatedBus = true;
                updatedGnd = true;
            }
            else Debug.WriteLine("Nothing to update");
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            disconnectAll();

            updatedGnd = true;
            updatedBus = true;
            communicator.writeSerialPort((int)Communicator.Functions.RESET + ";");

            InitializeComponent();
            DataContext = this;

            communicator.initSerialPort();
            updatedGnd = true;
            updatedBus = true;
        }
    }
}
