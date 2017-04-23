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
            Tool.CodeHelper.ProgramLan lan = (Tool.CodeHelper.ProgramLan)value;
            return lan == (Tool.CodeHelper.ProgramLan)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (Tool.CodeHelper.ProgramLan)int.Parse(parameter.ToString());
        }
    }
}
