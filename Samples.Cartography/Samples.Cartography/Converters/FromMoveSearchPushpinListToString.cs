using Samples.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Views
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
