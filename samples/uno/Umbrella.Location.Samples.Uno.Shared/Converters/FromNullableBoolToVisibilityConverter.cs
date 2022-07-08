using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#if NETFX_CORE
using Windows.UI.Xaml;
#elif __ANDROID__ || __IOS__
using Visibility = Windows.UI.Xaml.Visibility;
#else
using System.Windows;
#endif


namespace Umbrella.Location.Samples.Uno.Converters
{
    public class FromNullableBoolToVisibilityConverter : ConverterBase
	{
		public FromNullableBoolToVisibilityConverter()
		{
			this.VisibilityIfTrue = Converters.VisibilityIfTrue.Visible;
		}

		public VisibilityIfTrue VisibilityIfTrue { get; set; }

		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Not for end user")]
		protected override object Convert(object value, Type targetType, object parameter)
		{
			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			bool inverse = this.VisibilityIfTrue == VisibilityIfTrue.Collapsed;

			Visibility visibilityOnTrue = (!inverse) ? Visibility.Visible : Visibility.Collapsed;
			Visibility visibilityOnFalse = (!inverse) ? Visibility.Collapsed : Visibility.Visible;

			if (value != null && !(value is bool))
			{
				throw new ArgumentException($"Value must either be null or of type bool. Got {value} ({value.GetType().FullName})");
			}

			var valueToConvert = value != null && System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

			return valueToConvert ? visibilityOnTrue : visibilityOnFalse;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Not for end user")]
		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			if (value == null)
			{
				return null;
			}

			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			bool inverse = this.VisibilityIfTrue == VisibilityIfTrue.Collapsed;

			Visibility visibilityOnTrue = (!inverse) ? Visibility.Visible : Visibility.Collapsed;

			var visibility = (Visibility)value;

			return visibilityOnTrue.Equals(visibility);
		}
	}

	public enum VisibilityIfTrue
	{
		Visible,
		Collapsed
	}

}
