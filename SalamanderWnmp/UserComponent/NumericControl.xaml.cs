using System;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace SalamanderWnmp.UserComponent
{
	public partial class NumericControl : INotifyPropertyChanged
	{

		public NumericControl()
		{
			this.InitializeComponent();
		}

   
        public double Increment { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }




        /// <summary>
        /// 依赖属性
        /// </summary>
        [Description("获取或设置Value")]


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericControl));



        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue = (Value + Increment);
            if (newValue > MaxValue)
            {
                Value = MaxValue;
            }
            else
            {
                Value = newValue;
            }
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            double newValue = (Value - Increment);
            if (newValue < MinValue)
            {
                Value = MinValue;
            }
            else
            {
                Value = newValue;
            }
        }

        private void ValueText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Value = double.Parse(ValueText.Text);
            }
            catch (Exception)
            {
                Value = 0;
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void ValueText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void ValueText_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }



    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //double myValue = (double)value;
            //return myValue.ToString();
            try
            {
                return value.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //string myValue = (string)value;
            //return double.Parse(myValue);
            try
            {
                return double.Parse((string)value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
    }
}