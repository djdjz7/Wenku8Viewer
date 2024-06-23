using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;

namespace Wenku8Viewer.Converters
{
    public class MultiplierConverter : IValueConverter
    {
        public object? Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            decimal valueDecimal = 0;
            decimal parameterDecimal = 0;
            var result =
                decimal.TryParse(value?.ToString(), out valueDecimal)
                && decimal.TryParse(parameter?.ToString(), out parameterDecimal);
            if (result)
                return (double)(valueDecimal * parameterDecimal);
            return value;
        }

        public object? ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            decimal valueDecimal = 0;
            decimal parameterDecimal = 0;
            var result =
                decimal.TryParse(value?.ToString(), out valueDecimal)
                && decimal.TryParse(parameter?.ToString(), out parameterDecimal);
            if (result)
                return (double)(valueDecimal / parameterDecimal);
            return value;
        }
    }
}
