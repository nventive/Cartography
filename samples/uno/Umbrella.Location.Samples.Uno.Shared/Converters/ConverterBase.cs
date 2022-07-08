using System;
using System.Reflection;

#if NETFX_CORE
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using GenericCulture = System.String;
#elif __ANDROID__ || __IOS__
using Windows.UI.Xaml.Data;
using GenericCulture = System.String;
#else
using System.Windows.Data;
using GenericCulture = System.Globalization.CultureInfo;
#endif
namespace Umbrella.Location.Samples.Uno.Converters
{
	public abstract class ConverterBase : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, GenericCulture culture)
		{
			return Convert(value, targetType, parameter);
		}

		protected abstract object Convert(object value, Type targetType, object parameter);

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, GenericCulture culture)
		{
			return ConvertBack(value, targetType, parameter);
		}

		protected virtual object ConvertBack(object value, Type targetType, object parameter)
		{
			throw new NotSupportedException();
		}
	}
}
