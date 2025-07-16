using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace InkHouse.Views
{
    public class DateDisplayConverter : IValueConverter
    {
        public static readonly DateDisplayConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // ���������Ĭ��ֵ��DateTime.MinValue������ʾ"����"
                if (dateTime == DateTime.MinValue)
                {
                    return "����";
                }
                
                // ����ָ����ʽ��ʾ����
                string format = parameter as string ?? "yyyy-MM-dd";
                return dateTime.ToString(format);
            }
            
            return "����";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 