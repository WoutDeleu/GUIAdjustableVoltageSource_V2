using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System;

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

        // Start app
        public MainWindow()
        {
            InitializeComponent();
            ComSelector.SelectedIndex = 0;
            DataContext = this;

            stopwatch = Stopwatch.StartNew();

            communicator = new Communicator();

            InitializeMainWindow();
        }
        private void InitializeMainWindow()
        {
            // Clear textboxes logs
            CommandInterface.SelectAll();
            CommandInterface.Selection.Text = "";
            Status.SelectAll();
            Status.Selection.Text = "";
            Registers.SelectAll();
            Registers.Selection.Text = "";

            StatusBox_Status = "Git gelukt";
            communicator.InitSerialPort();

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
                try
                {
                    // Loop until communication is established
                    string message = "";
                    bool started = false, finished = false;
                    Stopwatch startupTimer = new Stopwatch();
                    startupTimer.Start();
                    while (!started)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 5000) throw new Exception("TIMEOUT: can't START setup communication Arduino.");
                        message += communicator.serialPort.ReadExisting();
                        if (message.Contains("##Setup Arduino##")) started = true;
                    }
                    StatusBox_Status = "Setup Arduino started";
                    while (!finished && startupTimer.ElapsedMilliseconds < 5000)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 10000) throw new Exception("TIMEOUT: can't FINISH setup communication Arduino.");
                        message += communicator.serialPort.ReadExisting();
                        if (message.Contains("##Setup Complete##")) finished = true;
                    }
                    StatusBox_Status = "Setup Arduino finished";
                    Home.IsEnabled = true;
                    Measure.IsEnabled = true;
                    BoardNumberSettings.IsEnabled = true;

                    Home.IsEnabled = true;
                    Measure.IsEnabled = true;
                    BoardNumberSettings.IsEnabled = true;

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
                catch (Exception ex)
                {
                    StatusBox_Error = ex.Message.ToString() + " Check if communication is correct and if Arduino is available.";

                    Home.IsEnabled = false;
                    Measure.IsEnabled = false;
                    BoardNumberSettings.IsEnabled = false;
                    tabcontroller.SelectedIndex = 3;

                    Current_Com.SetBinding(ContentProperty, new Binding("CurrentComPort"));
                }
            }
        }
        public void CloseMainWindow()
        {
            communicator.CloseSerialPort();
            Close();
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            Reset();
            e.Handled = true;
        }
    }
}
