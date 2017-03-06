using System;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Controls;

namespace SalamanderWnmp.UserComponent
{
	public partial class NumericControl : INotifyPropertyChanged
	{

		public NumericControl()
		{
			this.InitializeComponent();
		}

   
        public int Increment { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }




        /// <summary>
        /// 依赖属性
        /// </summary>
        [Description("获取或设置Value")]


        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumericControl));



        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            int newValue = (Value + Increment);
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
            int newValue = (Value - Increment);
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
                Value = int.Parse(ValueText.Text);
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

        private void ValueText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;

        }

        private void ValueText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int inputNum = 0;
            try
            {
                inputNum = int.Parse(e.Text);
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void ValueText_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            // 取消粘贴
            e.CancelCommand();
        }

        private void ValueText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                int num = int.Parse(textBox.Text);
                if(num > MaxValue || num < MinValue)
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }
    }



    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string myValue = (string)value;
            //return double.Parse(myValue);
            try
            {
                return int.Parse((string)value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
    }
}