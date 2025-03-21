﻿using System;
using Android.Runtime;
using Com.Nostra13.Universalimageloader.Core;
using Microsoft.UI.Xaml.Media;

namespace Sample.Droid;

[global::Android.App.ApplicationAttribute(
	Label = "@string/ApplicationName",
	Icon = "@mipmap/ic_launcher",
	LargeHeap = true,
	HardwareAccelerated = true,
	Theme = "@style/AppTheme",
	AllowBackup = false,
	ResizeableActivity = false
)]
public sealed class Application : Microsoft.UI.Xaml.NativeApplication
{
	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new App(), javaReference, transfer)
	{
		ConfigureUniversalImageLoader();
	}

	private static void ConfigureUniversalImageLoader()
	{
		// Create global configuration and initialize ImageLoader with this configuration.
		ImageLoaderConfiguration config = new ImageLoaderConfiguration
			.Builder(Context)
			.Build();

		ImageLoader.Instance.Init(config);

		ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
	}
}
