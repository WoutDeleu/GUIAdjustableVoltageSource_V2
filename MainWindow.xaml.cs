using System;
using System.IO;
using System.Windows;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Threading;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        delegate void serialCalback(string val);
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event 
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
            DataContext = this;

            // Initialize timer and communication with arduino
            communicator = new();
            communicator.initSerialPort(); 
            stopwatch = Stopwatch.StartNew();

            // Clear textboxes logs
            CommandInterface.SelectAll();
            CommandInterface.Selection.Text = "";
            Status.SelectAll();
            Status.Selection.Text = "";
            Registers.SelectAll();
            Registers.Selection.Text = "";

            // Loop until communication is established
            string message = "";
            bool started = false, finished = false;
            while(!started)
            {
                message += communicator.serialPort.ReadExisting();
                if (message.Contains("##Setup Arduino##")) started = true;
            }
            while (!finished )
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
            DataContext = this;
        }
        public void CloseMainWindow()
        {
            communicator.closeSerialPort();
            Close();
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            DisconnectAll();
            communicator.writeSerialPort((int)Communicator.Functions.RESET + ";");

            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }
    }
}
