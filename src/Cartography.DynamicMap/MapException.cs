using System;

namespace Cartography.DynamicMap;

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
