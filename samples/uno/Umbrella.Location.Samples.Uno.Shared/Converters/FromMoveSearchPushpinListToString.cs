using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.Location.Samples.Uno.Converters
{
	public class FromMoveSearchPushpinListToString : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			if (value is PushpinEntity[] pins
				&& pins.Any())
			{
				return $"{pins.Length} pushpins";
			}

			return "No pushpins";
		}
	}
}
