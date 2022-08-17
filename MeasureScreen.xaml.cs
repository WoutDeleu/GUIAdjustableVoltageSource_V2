using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public MeasureScreen()
        {
            InitializeComponent(); 
            Current_MeasuredValue.SetBinding(ContentProperty, new Binding("MeasuredValue"));
            DataContext = this;
            //
            // Initialisatie status connectToGround enz
            //
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
                //TODO
                //Measure Current
                MeasuredValue = 4 + "A";
            }                
            //TODO
            //Measure Current
            else
            {
                MeasuredValue = 8 + "V";
            }
        }
        private Boolean isConnectedToBus(int i)
        {
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
