using System.Globalization;

namespace Cartography.DynamicMap;

/// <summary>
/// Represents a zoom level on a map
/// </summary>
public struct ZoomLevel
{
	#region Operators

	/// <summary>
	/// Convert a zoom level to its raw value
	/// </summary>
	public static explicit operator double(ZoomLevel zoomLevel)
	{
		return zoomLevel.Value;
	}

	/// <summary>
	/// Create a zoom level from a raw value
	/// </summary>
	public static explicit operator ZoomLevel(double zoomLevel)
	{
		return new ZoomLevel(zoomLevel);
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator >(ZoomLevel left, ZoomLevel right)
	{
		return left.Value > right.Value;
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator <(ZoomLevel left, ZoomLevel right)
	{
		return left.Value < right.Value;
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator >=(ZoomLevel left, ZoomLevel right)
	{
		return left.Value >= right.Value;
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator <=(ZoomLevel left, ZoomLevel right)
	{
		return left.Value <= right.Value;
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator ==(ZoomLevel left, ZoomLevel right)
	{
		return left.Value == right.Value;
	}

	/// <summary>
	/// Compare raw values of two zoom levels
	/// </summary>
	public static bool operator !=(ZoomLevel left, ZoomLevel right)
	{
		return left.Value != right.Value;
	}
	#endregion

	private readonly double _value;

	/// <summary>
	/// Create a custom zoom level based a double value.
	/// </summary>
	/// <remarks>Consider using one the preconfigured <see cref="ZoomLevels"/>.</remarks>
	public ZoomLevel(double value)
	{
		_value = value;
	}

	/// <summary>
	/// The raw value of the zoom level.
	/// </summary>
	public double Value
	{
		get { return _value; }
	}

	public override bool Equals(object obj)
	{
		return obj is ZoomLevel && this == (ZoomLevel)obj;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return Value.ToString(CultureInfo.InvariantCulture);
	}
}
