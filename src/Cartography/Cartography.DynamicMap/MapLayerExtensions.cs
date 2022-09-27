#if __IOS__ || __ANDROID__ || WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cartography.DynamicMap
{
	public static partial class MapLayerExtensions
	{
		private static readonly IEqualityComparer<IGeoLocated> _comparer = EqualityComparer<IGeoLocated>.Default;

		/// <remarks>Does not ensure to keep items ordering in the pusphin map layer.</remarks>
		public static void Update<TLayer, TPin>(
			this TLayer layer, 
			IGeoLocated[] items, 
			IGeoLocated[] selectedItems,
			Func<IGeoLocated, TPin> containerFactory,
			bool canRecycle = true)
			where TLayer : IMapLayer<TPin>
			where TPin : IMapControlItem, ISelectable
		{
			if (items.Length == 0)
			{
				layer.Clear();
				return;
			}

			var containers = layer
				.Items
				.ToDictionary(container => container.Item, container => container, _comparer);

			var removedItems = containers.Keys.Except(items, _comparer).ToArray();
			var addedItems = items.Except(containers.Keys, _comparer).ToArray();

			// Remove overlays
			var recycledOverlays = removedItems
				.Select(item => containers[item])
				.Where(layer.Remove)
				.ToList();

			// Insert new Items
			var lowerIndex = layer.Count();
			var upperIndex = lowerIndex + addedItems.Count() - 1;
			using (var recyclingProvider = recycledOverlays.GetEnumerator())
			{
				foreach (var item in addedItems)
				{
					var container = canRecycle && recyclingProvider.MoveNext()
						? recyclingProvider.Current
						: containerFactory(item);

					container.Item = item;
					container.UpdateSelectionFrom(selectedItems);

					var index = container.IsSelected
						? upperIndex--
						: lowerIndex++;
					layer.Insert(index, container);
				}
			}

			// Ensure inserted items don't cover existing selected items
			layer.UpdateZIndex();
		}

		public static void UpdateSelection<TPin>(
			this IMapLayer<TPin> layer,
			IGeoLocated[] items)
			where TPin : ISelectable
		{
			foreach (var container in layer.Items)
			{
				container.UpdateSelectionFrom(items);
			}
			layer.UpdateZIndex();
		}

		private static void UpdateZIndex<TPin>(this IMapLayer<TPin> layer)
			where TPin : ISelectable
		{
			var lowerIndex = 0;
			var upperIndex = layer.Count() - 1;
			foreach (var item in layer.Items)
			{
				item.ZIndex = item.IsSelected
					? upperIndex--
					: lowerIndex++;
			}
		}

		private static void UpdateSelectionFrom(this ISelectable pushpin, IGeoLocated[] selected)
		{
			pushpin.IsSelected = pushpin.GetItems().Intersect(selected, _comparer).Any();
		}

		private static IEnumerable<IGeoLocated> GetItems(this IMapControlItem container)
		{
			var group = container.Item as IGeoLocatedGrouping;
			if (group == null)
			{
				yield return container.Item as IGeoLocated;
			}
			else
			{
				foreach (var item in group)
				{
					yield return item as IGeoLocated;
				}
			}
		}
	}
}
#endif
