namespace Cartography;

public partial interface IMapControlItem
{
	IGeoLocated Item { get; set; }
}

public partial interface ISelectable : IMapControlItem
{
	bool IsSelected { get; set; }

	int ZIndex { get; set; }
}
