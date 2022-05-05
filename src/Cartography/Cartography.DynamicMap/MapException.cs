using System;
using System.Collections.Generic;
using System.Text;

namespace Nventive.Location.DynamicMap
{
	public partial class MapException : Exception
	{
		/// <summary>
		/// Ctor
		/// </summary>
		public MapException(Exception inner)
			: base("Common map exception", inner)
		{
		}
	}
}
