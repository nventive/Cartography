using Cartography.DynamicMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Samples.Entities
{
    class PushpinsEntityGroup : IGeoLocatedGrouping<PushpinEntity[]>
    {
        private List<PushpinEntity[]> _pushpinCollections = new List<PushpinEntity[]>();

        public Geopoint Coordinates { get; set; }

        public Geocoordinate Key { get; set; }

        public void Add(PushpinEntity[] pushpins)
        {
            _pushpinCollections.Add(pushpins);
        }

        public IEnumerator<PushpinEntity[]> GetEnumerator()
        {
            return _pushpinCollections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
