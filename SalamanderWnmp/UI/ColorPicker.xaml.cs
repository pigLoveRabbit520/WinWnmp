using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SalamanderWnmp.UI
{
    /// <summary>
    /// ColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPicker : System.Windows.Controls.UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            SetUpCommands();
        }

        private void SetUpCommands()
        {
            // 设置命令关联，一般的方法
            CommandBinding binding = new CommandBinding(ApplicationCommands.Undo,
             UndoCommand_Executed, UndoCommand_CanExecute);
            this.CommandBindings.Add(binding);
        }

        // 存储之前的颜色
        private SolidColorBrush previousColor;


        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;
            e.CanExecute = (colorPicker.previousColor != null);
        }
        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // WPF冻结的问题
            if (previousColor.CanFreeze)
            {
                previousColor.Freeze();
            }
            ColorPicker colorPicker = (ColorPicker)sender;
            colorPicker.Color = (SolidColorBrush)colorPicker.previousColor;
            //e.Handled = true;
        }

        //依赖属性包装器，分别对应R，G，B，Alpha通道和Color颜色
        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }
        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }




        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }        

        


        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        

        


        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register("Red", typeof(byte), typeof(ColorPicker),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRGBAlphaChanged)));


        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register("Green", typeof(byte), typeof(ColorPicker),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRGBAlphaChanged)));


        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register("Blue", typeof(byte), typeof(ColorPicker),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRGBAlphaChanged)));


        public static readonly DependencyProperty AlphaProperty =
              DependencyProperty.Register("Alpha", typeof(byte), typeof(ColorPicker),
              new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRGBAlphaChanged)));


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(ColorPicker),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorChanged)));





        // 关联的影响方法
        private static void OnRGBAlphaChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;
            SolidColorBrush newColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                colorPicker.Alpha,colorPicker.Red,colorPicker.Green,colorPicker.Blue));
            colorPicker.Color = newColor;
        }

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)sender;

            SolidColorBrush oldColor = (SolidColorBrush)e.OldValue;
            SolidColorBrush newColor = (SolidColorBrush)e.NewValue;

            colorPicker.Red = newColor.Color.R;
            colorPicker.Green = newColor.Color.G;
            colorPicker.Blue = newColor.Color.B;
            colorPicker.Alpha = Convert.ToByte(newColor.Color.ToString().Substring(1, 2),16);//变通方法而已

            colorPicker.previousColor = oldColor;
            colorPicker.OnColorChanged(oldColor, newColor);
        }


        // 路由事件
        public static readonly RoutedEvent ColorChangedEvent =
           EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<SolidColorBrush>), typeof(ColorPicker));

        public event RoutedPropertyChangedEventHandler<SolidColorBrush> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        // 激发路由事件
        private void OnColorChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
        {
            RoutedPropertyChangedEventArgs<SolidColorBrush> args = new RoutedPropertyChangedEventArgs<SolidColorBrush>(oldValue, newValue);
            args.RoutedEvent = ColorPicker.ColorChangedEvent;
            RaiseEvent(args);
        }
    }
}
