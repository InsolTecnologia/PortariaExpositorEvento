using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.IO;

namespace PortariaExpositorEvento.Converter;

public class ImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string imageName)
        {
            var fileExtension = Path.GetExtension(imageName).ToLowerInvariant();
            
            var uri = new Uri($"avares://PortariaExpositorEvento/Assets/{imageName}");

            using var assetStream = AssetLoader.Open(uri);

            return new Bitmap(assetStream);
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
