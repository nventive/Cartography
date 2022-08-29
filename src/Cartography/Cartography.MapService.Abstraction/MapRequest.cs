using Windows.Devices.Geolocation;

namespace Cartography.MapService
{
	/// <summary>
	/// This class aggregates map parameters.
	/// </summary>
	public class MapRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MapRequest"/> class.
		/// </summary>
		/// <param name="coordinates">Coordinates</param>
		/// <param name="label">Label</param>
		/// <param name="userLocation">UserLocation</param>
		public MapRequest(BasicGeoposition coordinates, BasicGeoposition userLocation = default, string label = null)
		{
			Coordinates = coordinates;
			UserLocation = userLocation;
			Label = label;
			IsCoordinatesSet = true;
			IsUserLocationSet = !userLocation.Equals(default(BasicGeoposition));
		}

        public MapRequest(BasicGeoposition coordinates, string label = null)
        {
            Coordinates = coordinates;
            UserLocation = default(BasicGeoposition);
            Label = label;
            IsCoordinatesSet = true;
            IsUserLocationSet = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapRequest"/> class.
        /// </summary>
        /// <param name="locationName">LocationName</param>
        public MapRequest(string locationName)
		{
			LocationName = locationName;
		}

		/// <summary>
		/// Gets the coordinates of the destination.
		/// </summary>
		public BasicGeoposition Coordinates { get; }

        /// <summary>
        /// Gets the coordinates of the User.
        /// </summary>
        public BasicGeoposition UserLocation { get; }

		/// <summary>
		/// Gets if Coordinates are set.
		/// </summary>
		public bool IsCoordinatesSet { get; }

		/// <summary>
		/// Gets if user location are set.
		/// </summary>
		public bool IsUserLocationSet { get; }

		/// <summary>
		/// Gets the label.
		/// </summary>
		public string Label { get; }

		/// <summary>
		/// Gets the location name.
		/// </summary>
		public string LocationName { get; }
	}
}
