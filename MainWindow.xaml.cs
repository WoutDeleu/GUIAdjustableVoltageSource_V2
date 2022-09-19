using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System;
using System.Windows.Threading;

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

        // Communicator with the arduino
        Communicator Communicator;
        BrushConverter Bc = new();
        // Keep track of time which app is opened
        Stopwatch AppTimer;
        DispatcherTimer RefreshArduinoStatus;
        DispatcherTimer RefreshCurrentMeasure;

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

            Communicator = new();

            InitializeCommunication();

            RefreshArduinoStatus = new()
            {
                Interval = TimeSpan.FromSeconds(8)
            };
            RefreshArduinoStatus.Tick += UpdateArduinoStatus;


            DispatcherTimer RefreshCurrentMeasure = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            RefreshCurrentMeasure.Tick += UpdateMeasuredCurrent;
            MeasuredCurrentPeriodResult.Text = "Not Yet Set";

            RefreshCurrentMeasure.Start();
            RefreshArduinoStatus.Start();

            SelectionVisible = Visibility.Visible;
            MeasuredResult.SetBinding(ContentProperty, new Binding("MeasuredValue"));

            CurrentBoardNumber.SetBinding(ContentProperty, new Binding("BoardNumber"));
            GetBoardNumberArduino();

            currentCOM.SetBinding(ContentProperty, new Binding("currentCOMPort"));
        }
        public void CloseMainWindow()
        {
            Communicator.CloseSerialPort();
            Close();
        }
        public void Reset(object sender, RoutedEventArgs e)
        {
            Reset();
            e.Handled = true;
        }
    }
}
