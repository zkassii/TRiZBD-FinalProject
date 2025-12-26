using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ShoeStoreWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var borderStyle = new Style(typeof(Border));
            borderStyle.Setters.Add(new Setter(Border.BackgroundProperty,
                new Binding("Background") { Converter = new StringToBrushConverter() }));
            Resources[typeof(Border)] = borderStyle;
        }
    }

    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorStr && !string.IsNullOrWhiteSpace(colorStr))
            {
                try { return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr)); }
                catch { }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
    }
}