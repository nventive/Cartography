#if WINDOWS
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// An item which can be displayed in a Map layer
	/// </summary>
	public class MapControlItem : ContentControl, IMapControlItem
	{
		internal MapControlItem(MapControlBase map)
		{
			Map = map;
		}

#region PositionOrigin (dp)
		/// <summary>
		/// Identifies the <see cref="PositionOrigin"/> dependency property.
		/// </summary>

		public static readonly DependencyProperty PositionOriginProperty = DependencyProperty.Register(
			"PositionOrigin", typeof(Point), typeof(MapControlItem), new PropertyMetadata(default(Point), OnPositionOriginChanged));

		/// <summary>
		/// Relative origin of the content
		/// </summary>
		public Point PositionOrigin
		{
			get { return (Point)GetValue(PositionOriginProperty); }
			set { SetValue(PositionOriginProperty, value); }
		}

		private static void OnPositionOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Windows.UI.Xaml.Controls.Maps.MapControl.SetNormalizedAnchorPoint(d, (Point)e.NewValue);
		} 
#endregion

		IGeoLocated IMapControlItem.Item
		{
			get { return (IGeoLocated) Content; }
			set { Content = value; }
		}
		internal IGeoLocated Item
		{
			get { return (IGeoLocated) Content; }
			set { Content = value; }
		}

		/// <summary>
		/// Gets the source <see cref="MapControlBase"/> of this item.
		/// </summary>
		public MapControlBase Map { get; private set; }

		/// <summary>
		/// Update the location of the item on the map.
		/// </summary>
		/// <param name="coordinates">Location of the item</param>
		public void UpdateLocation(Geopoint coordinates)
		{
			Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(this, coordinates);
		}

		/// <inherit />
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			UpdateLocation(((IGeoLocated)newContent).Coordinates);

			base.OnContentChanged(oldContent, newContent);
		}
	}
}
#endif
