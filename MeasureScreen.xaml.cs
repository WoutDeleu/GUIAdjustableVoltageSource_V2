using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
        public static Tierce tierce;
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
        public MeasureScreen(Tierce s)
        {
            InitializeComponent(); 
            Current_MeasuredValue.SetBinding(ContentProperty, new Binding("MeasuredValue"));
            DataContext = this;
            tierce = s;
        }
        private void CloseMeasureScreen(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MeasureValue(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(SelectMeasureFunction.SelectedItem.ToString());
            if (SelectMeasureFunction.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Measure current")
            {
                double current_out;
                tierce.writeSerialPort((int)Tierce.Functions.MEASURE_CURRENT + ";");

                String input ="";

                while(tierce.serialPort.BytesToRead != 0)
                {
                    input += tierce.serialPort.ReadExisting();
                }
                Debug.WriteLine(input);
                string current = extractInput(input).Replace(".", ",");
                if (double.TryParse(current, out current_out))
                    MeasuredValue = current_out + " A";
                else
                {
                    Debug.WriteLine("Fault in measure format...");
                    MeasuredValue = "FAULT";

                }
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
        private string extractInput(string s)
        {
            char[] array = s.ToCharArray();
            int begin = 0, end = 0, length ;
            for(int i=0; i<array.Length; i++)
            {
                if (array[i] == '[') begin = i;
                if (array[i] == ']') end = i;
            }
            if (end == 0)
                {
                Debug.WriteLine("FAULT IN COMMUNICATION");
                return "FAULT IN COMMUNICATION";
            }
            else
            {
                Debug.WriteLine(s.Substring(begin + 1, (end - begin - 1)));
                return s.Substring(begin + 1, (end - begin - 1));
            }
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
