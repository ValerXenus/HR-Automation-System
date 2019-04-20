using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace HR_Automation_System.Converters
{
    // Конвертер размеров TabItem для окна авторизации
    public class TabItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TabControl tabControl = values[0] as TabControl;
            double width = tabControl.ActualWidth / tabControl.Items.Count;
            // Вычитаем 2, чтобы не было переноса на другую строку
            return (width <= 1) ? 0 : (width - 2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // не используем
        }
    }
}
