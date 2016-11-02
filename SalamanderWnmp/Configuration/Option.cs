using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SalamanderWnmp.Configuration
{
    public interface IOption
    {
        void ReadIniValue(string IniFileStr);
        void Convert();
        void PrintIniOption(StreamWriter sw);
    }

    public class Option<T> : IOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string iniValue { get; set; }
        public T Value { get; set; }

        public void ReadIniValue(string IniFileStr)
        {
            string key = Name + "=";
            using (var sr = new StringReader(IniFileStr)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    if (line.StartsWith(key)) {
                        iniValue = line.Remove(0, key.Length);
                        return;
                    }
                }
            }
            iniValue = "";
        }

        public void PrintIniOption(StreamWriter sw)
        {
            sw.WriteLine("; " + Description);
            sw.WriteLine(Name + "=" + Value.ToString());
        }

        public void Convert()
        {
            if (iniValue == "")
                return;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null) {
                try {
                    Value = (T)converter.ConvertFromString(iniValue);
                } catch (Exception ex) {
                    // Could be made a bit more elegant but considering its a rare user-caused exception....
                    var message = String.Format("{0}={1}\n{2}\n\nThe Default Value '{3}' will be used instead.",
                        Name, iniValue, ex.Message, Value.ToString());
                    MessageBox.Show(message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
