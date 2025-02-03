using System.Collections.Generic;

namespace Cartography;

public interface IMapLayer<TItem>
	where TItem : IMapControlItem
{
	IEnumerable<TItem> Items { get; }
	int Count();
	void Clear();
	void Insert(int index, TItem item);
	bool Remove(TItem item);
	void Add(TItem item);
	TItem ElementAt(int index);
	void RemoveAt(int index);
	bool Contains(TItem item);
}