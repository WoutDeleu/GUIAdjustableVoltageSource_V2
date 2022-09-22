using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using System.IO.Ports;
using System.Threading;

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

            if(IsConnectionSuccesfull)
            {
                try
                {
                    // Loop until communication is established
                    string message = "";
                    bool started = false, finished = false;

                    // Setup Arduino, and wait for correct response
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

                    // Check if session is restored
                    started = false;
                    finished = false; 
                    message = "";
                    while (!started)
                    {
                        message += serialPort.ReadExisting();
                        if (startupTimer.ElapsedMilliseconds >= 20000) throw new Exception("TIMEOUT: can't START setup communication Arduino. (Restoring session not started)");
                        if (message.Contains("##Restoring session##")) started = true;
                        else if (message.Contains("##Not Restoring session##")) started = finished = true;
                    }
                    while (!finished)
                    {
                        message += serialPort.ReadExisting();
                        if (startupTimer.ElapsedMilliseconds >= 25000) throw new Exception("TIMEOUT: can't FINISH setup communication Arduino. Message Received: " + message);
                        if (message.Contains("##Finished Restoring session")) finished = true;
                    }
                    do
                    {
                        if (serialPort.IsOpen) message += serialPort.ReadExisting();
                    } while (serialPort.IsOpen && serialPort.BytesToRead != 0);
                    FilterInput(message);
                    StatusBox_Status = "Setup Arduino finished";

                    RestoreSession();

                    UpdateBoardNumber();

                    DataContext = this;
                }
                // Time Out Exception
                catch (Exception ex)
                {
                    StatusBox_Error = ex.Message.ToString() + " Check if communication is correct and if Arduino is available.";
                    StatusBox_Error = "Port " + serialPort.PortName + " could not be opened";

                    CloseSerialPort();
                    IsConnectionSuccesfull = false;
                }
            }
        }

        public void SetUpBindings()
        {
            MeasuredResult.SetBinding(ContentProperty, new Binding("MeasuredValue"));
            CurrentBoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
            currentCOM.SetBinding(ContentProperty, new Binding("CurrentCOMPort"));
        }
        
        public void CloseMainWindow()
        {
            CloseSerialPort();
            Close();
        }
        public void ResetButton(object sender, RoutedEventArgs e)
        {
            ResetBasedOnCOM();
            e.Handled = true;
        }
        
        // Retrieve previous settings
        public void RestoreSession()
        {
            MeasuredCurrentPeriodResult.Text = MeasureCurrent();
            // RetrievePreviousState();

        }
        public void RetrievePreviousState()
        {
            WriteSerialPort((int)BoardFunctions.GET_PEVIOUS_STATE + ";");

            string input = "";
            do
            {
                input += serialPort.ReadExisting();
            } while (serialPort.BytesToRead != 0);
            FilterInput(input);
            string[] inputArr = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            int counter = 0;
            foreach(string str in inputArr)
            {
                if(str.Contains("["))
                {
                    if(counter == 0)
                    {
                        if (int.Parse(ExtractInput(str)) == 0)
                        {
                            StatusBox_Status = "Recover data flag = false. Previous data was not saved";
                            break;
                        }
                        else
                        {
                            StatusBox_Status = "Recover data flag = ture. Recover previous state";
                            IsConnectedToBus_1 = true;
                            counter++;
                        }
                    }
                    if(counter == 1)
                    {

                        Voltage = double.Parse(ExtractInput(str));
                        SetVoltageTextBox.Text = ExtractInput(str);
                        counter++;
                    }
                }
                if (counter == 2) break;
            }
            if (counter > 2) StatusBox_Error = "Something went completetly wrong.";
        }
    }
}
