using System;

namespace Cartography;

public partial class MapException : Exception
{
	public MapException(Exception inner)
		: base("Common map exception", inner)
	{
	}
}
