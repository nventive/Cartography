using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Chinook.DynamicMvvm;

namespace Nventive.Location.DynamicMap
{
	internal static class MapComponentExtensions
	{
		public static IObservable<IGeoLocated[]> GetAndObservePushpins(this ViewModelBase vm, object filterOutChangesFromSource)
		{
			var component = (IMapComponent)vm;
			var valueChanged = vm.GetProperty<IGeoLocated[]>(nameof(component.Pushpins)).GetAndObserve();

			return (IObservable<IGeoLocated[]>)valueChanged
				.Where(message => message != filterOutChangesFromSource)
				.Select(message => AsGeoLocatedItems(message))
				.StartWith(new IGeoLocated[] { });
		}

		public static IObservable<IGeoLocated[]> GetAndObserveGroups(this ViewModelBase vm, object filterOutChangesFromSource)
		{
			var component = (IMapComponent)vm;
			var valueChanged = vm.GetProperty<IGeoLocatedGrouping<IGeoLocated[]>>(nameof(component.Groups)).GetAndObserve();

			return valueChanged
				.Where(message => message != filterOutChangesFromSource)
				.Select(message => AsGeoLocatedItems(message))
				.StartWith(new IGeoLocated[] { });
		}

		public static IObservable<IGeoLocated[]> GetAndObserveSelectedPushpins(this ViewModelBase vm, object filterOutChangesFromSource = null)
		{
			var component = (IMapComponent)vm;
			var valueChanged = vm.GetProperty<IGeoLocated[]>(nameof(component.SelectedPushpins)).GetAndObserve();


			if (filterOutChangesFromSource != null)
			{
				valueChanged = valueChanged.Where(message => message != filterOutChangesFromSource);
			}

			return valueChanged
				.Select(message => AsGeoLocatedItems(message))
				.StartWith(new IGeoLocated[] { });
		}

		private static IGeoLocated[] AsGeoLocatedItems(object value)
		{
			return value as IGeoLocated[]
				?? ((value as IEnumerable) ?? Enumerable.Empty<object>())
					.OfType<IGeoLocated>()
					.Distinct(EqualityComparer<IGeoLocated>.Default)
					.ToArray();
		}
	}
}
