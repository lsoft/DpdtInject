using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DpdtInject.Extension.Helper
{
    public sealed class GridViewConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            var scale = int.Parse(parameter as string) / 100.0;

            var l = o as ListView;

            if (l is null)
            {
                return 1;
            }

            return
                l.ActualWidth * scale * /*0.945*/0.975;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
