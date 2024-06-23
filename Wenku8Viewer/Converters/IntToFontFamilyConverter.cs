using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;

namespace Wenku8Viewer.Converters;

public class IntToFontFamilyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int x && x == 0)
        {
            var sourceHanSerifCN = Application.Current?.Resources["SourceHanSerifSC"];
            if (sourceHanSerifCN is FontFamily)
                return sourceHanSerifCN;
        }
        return FontFamily.Parse("Inter, Microsoft YaHei");
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotImplementedException();
    }
}
