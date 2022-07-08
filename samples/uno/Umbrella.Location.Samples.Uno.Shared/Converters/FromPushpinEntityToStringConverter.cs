using System;
using System.Collections.Generic;
using System.Text;
using Uno.Extensions;
using Windows.UI.Xaml.Data;

namespace Umbrella.Location.Samples.Uno.Converters
{
	public class FromPushpinEntityToStringConverter : IValueConverter
	{
		public string DefaultValue { get; set; }

		public string SelectedValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var store = value as PushpinEntity;

#if __ANDROID__
			var isSelected = parameter as bool? ?? false;
#else
			var isSelected = store.SafeEquals(parameter as PushpinEntity);
#endif
			return isSelected ? SelectedValue : DefaultValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
