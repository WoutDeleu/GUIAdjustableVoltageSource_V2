using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdjustableVoltageSource
{

    public partial class MeasureScreen : Window, INotifyPropertyChanged
    {
        public static SerialPort serialPort;
        private string _measuredValue;
        public string MeasuredValue
        {
            get { return _measuredValue; }
            set
            {
                if (value != _measuredValue)
                {
                    _measuredValue = value;
                    OnPropertyChanged("MeasuredValue");
                }
            }
        }
        public MeasureScreen(SerialPort s)
        {
            InitializeComponent(); 
            Current_MeasuredValue.SetBinding(ContentProperty, new Binding("MeasuredValue"));
            DataContext = this;
            serialPort = s;
        }
        private void CloseMeasureScreen(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MeasureValue(object sender, RoutedEventArgs e)
        {
			// TODO
            Debug.WriteLine(SelectMeasureFunction.SelectedItem.ToString());
            if (SelectMeasureFunction.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Measure current")
            {
                MeasuredValue = 4 + "A";
            }                

            else
            {
                MeasuredValue = 8 + "V";
            }
        }
        private Boolean isConnectedToBus(int i)
        {
			// TODO
            return true;
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var handler = System.Threading.Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
