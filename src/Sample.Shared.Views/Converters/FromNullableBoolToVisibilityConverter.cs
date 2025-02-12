using Microsoft.UI.Xaml;
using Nventive.View.Converters;
using System;
using System.Globalization;

namespace Sample.Views
{
	public class FromNullableBoolToVisibilityConverter : ConverterBase
	{
		public FromNullableBoolToVisibilityConverter()
		{
			this.VisibilityIfTrue = VisibilityIfTrue.Visible;
		}

		public VisibilityIfTrue VisibilityIfTrue { get; set; }

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