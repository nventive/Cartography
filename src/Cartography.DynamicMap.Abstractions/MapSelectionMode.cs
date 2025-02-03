namespace Cartography;

/// <summary>
/// An enum of the possible selection modes on a <see cref="MapControl"/>.
/// </summary>
public enum MapSelectionMode
{
	/// <summary>
	/// Selection disabled
	/// </summary>
	None,

	/// <summary>
	/// Only one pushpin selected at a time
	/// </summary>
	Single,

	/// <summary>
	/// Allow multiple pushpin selection
	/// </summary>
	Multiple,
}
