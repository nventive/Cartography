﻿#if __IOS__
using System;
using System.Runtime.CompilerServices;
using Google.Maps;

namespace Cartography.DynamicMap.GoogleMap.iOS;

public class GooglePushpin : GoogleMapControlItem, ISelectable
{
	private bool _isSelected;
	public bool IsSelected
	{
		get { return _isSelected; }
		set
		{
			if (_isSelected != value)
			{
				_isSelected = value;
				RaisePropertyChanged();
			}
		}
	}

	private int _zIndex;
	public int ZIndex
	{
		get => _zIndex;
		set
		{
			if (_zIndex != value)
			{
				_zIndex = value;
				RaisePropertyChanged();
			}
		}
	}

	public Action<GooglePushpin, Marker> MarkerUpdater { get; set; }

	protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
	{
		base.RaisePropertyChanged(propertyName);

		if (MarkerUpdater != null && Marker != null)
		{
			MarkerUpdater(this, Marker);
		}
	}
}
#endif
