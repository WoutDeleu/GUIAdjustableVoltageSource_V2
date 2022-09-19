using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using System.IO.Ports;

namespace AdjustableVoltageSource
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        delegate void serialCalback(string val);

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

        // Keep track of time which app is opened
        Stopwatch AppTimer;

        private bool IsGndUpdated { get; set; }
        private bool IsBusUpdated { get; set; }
        private double Voltage { get; set; }


        // Start app
        public MainWindow()
        {
            InitializeComponent();
            COMSelector.SelectedIndex = 0;
            DataContext = this;

            AppTimer = Stopwatch.StartNew();
            serialPort = new()
            {
                BaudRate = baudrate
            };

            InitializeCommunication();

            SetupPeriodicStatusses();

            SetUpBindings();
            SelectionVisible = Visibility.Visible;
        }

        // Establish communication with the arduino
        private void InitializeCommunication()
        {
            ClearTextboxes();

            InitSerialPort();
            labelCurrentCOM.Text = CurrentCOMPort;
            if (!isConnectionSuccesfull)
            {
                UpdateArduinoStatus(false);
            }
            else
            {
                try
                {
                    // Loop until communication is established
                    string message = "";
                    bool started = false, finished = false;
                    Stopwatch startupTimer = new();
                    startupTimer.Start();
                    while (!started)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 5000) throw new Exception("TIMEOUT: can't START setup communication Arduino.");
                        message += serialPort.ReadExisting();
                        if (message.Contains("##Setup Arduino##")) started = true;
                    }
                    StatusBox_Status = "Setup Arduino started";
                    while (!finished && startupTimer.ElapsedMilliseconds < 5000)
                    {
                        if (startupTimer.ElapsedMilliseconds >= 10000) throw new Exception("TIMEOUT: can't FINISH setup communication Arduino.");
                        message += serialPort.ReadExisting();
                        if (message.Contains("##Setup Complete##")) finished = true;
                    }
                    StatusBox_Status = "Setup Arduino finished";

                    // Enable correct tabs/update statusses
                    UpdateArduinoStatus(true);

                    UpdateBoardNumber();
                    DataContext = this;
                }
                catch (Exception ex)
                {
                    StatusBox_Error = ex.Message.ToString() + " Check if communication is correct and if Arduino is available.";
                    StatusBox_Error = "Port " + serialPort.PortName + " could not be opened";

                    CloseSerialPort();
                    UpdateArduinoStatus(false);
                }
            }
        }

        public void SetUpBindings()
        {
            MeasuredResult.SetBinding(ContentProperty, new Binding("MeasuredValue"));
            CurrentBoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
            currentCOM.SetBinding(ContentProperty, new Binding("currentCOMPort"));
        }
        public void CloseMainWindow()
        {
            CloseSerialPort();
            Close();
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            Reset();
            e.Handled = true;
        }
    }
}
