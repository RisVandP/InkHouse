using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace InkHouse.Views
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // ��countΪ0ʱ��ʾ"���޼�¼"����������
                return count == 0;
            }
            return true; // Ĭ����ʾ"���޼�¼"
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
} 