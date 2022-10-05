#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartography.DynamicMap
{
    /// <summary>
    /// Marker for the User Location
    /// </summary>
    public class UserLocationMarker
    {
        internal UserLocationMarker()
        {
            // Waiting for implementation of MapControl for WinUI3
            throw new NotImplementedException("Not Implemented for windows");
        }
    }
}
#endif
