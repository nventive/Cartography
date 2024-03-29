﻿#if __IOS__ || __ANDROID__ || WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cartography.DynamicMap
{
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
}
#endif
