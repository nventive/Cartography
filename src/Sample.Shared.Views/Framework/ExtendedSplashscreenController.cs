﻿using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace Sample;

public sealed class ExtendedSplashscreenController : IExtendedSplashscreenController
{
	private readonly DispatcherQueue _dispatcherQueue;

	public ExtendedSplashscreenController(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue;
	}

	public void Dismiss()
	{
		_ = _dispatcherQueue.EnqueueAsync(DismissSplashScreen);

		void DismissSplashScreen() // Runs on UI thread.
		{
			Shell.Instance.ExtendedSplashScreen.Dismiss();
		}
	}
}
