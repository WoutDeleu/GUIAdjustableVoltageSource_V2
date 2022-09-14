using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Controls;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        delegate void serialCalback(string val);
        BrushConverter bc = new BrushConverter();

        // Create the OnPropertyChanged method to raise the event 
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        // Communicator with the arduino
        Communicator communicator;
        // Keep track of time which app is opened
        Stopwatch stopwatch;

        private bool updatedGnd { get; set; }
        private bool updatedBus { get; set; }
        private double voltage { get; set; }

        // Start up app
        public MainWindow()
        {
            InitializeComponent();
            ComSelector.SelectedIndex = 0;
            DataContext = this;

            stopwatch = Stopwatch.StartNew();

            communicator = new();

            initializeMainWindow();
        }
        private void initializeMainWindow()
        {
            // Clear textboxes logs
            CommandInterface.SelectAll();
            CommandInterface.Selection.Text = "";
            Status.SelectAll();
            Status.Selection.Text = "";
            Registers.SelectAll();
            Registers.Selection.Text = "";

            communicator.initSerialPort();

            if (!communicator.connectionSuccesfull)
            {
                Home.IsEnabled = false;
                Measure.IsEnabled = false;
                BoardNumberSettings.IsEnabled = false;
                tabcontroller.SelectedIndex = 3;

                Current_Com.SetBinding(ContentProperty, new Binding("CurrentComPort"));
            }
            else
            {
                Home.IsEnabled = true;
                Measure.IsEnabled = true;
                BoardNumberSettings.IsEnabled = true;

                Home.IsEnabled = true;
                Measure.IsEnabled = true;
                BoardNumberSettings.IsEnabled = true;

                // Loop until communication is established
                string message = "";
                bool started = false, finished = false;
                while (!started)
                {
                    message += communicator.serialPort.ReadExisting();
                    if (message.Contains("##Setup Arduino##")) started = true;
                }
                while (!finished)
                {
                    message += communicator.serialPort.ReadExisting();
                    if (message.Contains("##Setup Complete##")) finished = true;
                }
                StatusBox_Status = "Setup Arduino started";
                StatusBox_Status = "Setup Arduino finished";

                // Bindings and default values
                updatedGnd = true;
                updatedBus = true;

                SelectionVisible = Visibility.Visible;
                Current_MeasuredValue.SetBinding(ContentProperty, new Binding("MeasuredValue"));

                Current_BoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
                GetBoardNumberArduino();

                Current_Com.SetBinding(ContentProperty, new Binding("CurrentComPort"));

                DataContext = this;
            }
        }
        public void CloseMainWindow()
        {
            communicator.closeSerialPort();
            Close();
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            Reset();
            tabcontroller.SelectedIndex = 0;
            e.Handled = true;
        }
    }
}
