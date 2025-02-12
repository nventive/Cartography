using System;
using System.Linq;
using Nventive.View.Converters;
using Sample.Entities;

namespace Sample.Views;

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
