using SalamanderWnmp.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SalamanderWnmp.Converter
{
    class LanToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CodePanel.ProgramLan lan = (CodePanel.ProgramLan)value;
            return lan == (CodePanel.ProgramLan)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (CodePanel.ProgramLan)int.Parse(parameter.ToString());
        }
    }
}
