using System;
using Samples.Entities;
using Uno.Extensions;

namespace Samples.Views
{
    public class FromPushpinEntityToStringConverter : ConverterBase
    {
        public string UnselectedValue { get; set; }

        public string SelectedValue { get; set; }

        protected override object Convert(object value, Type targetType, object parameter)
        {
            var Pushpin = value as PushpinEntity;

#if __ANDROID__
            var isSelected = parameter as bool? ?? false;
#else
			var isSelected = Pushpin.SafeEquals(parameter as PushpinEntity);
#endif

            return isSelected
                ? SelectedValue
                : UnselectedValue;
        }
    }
}
