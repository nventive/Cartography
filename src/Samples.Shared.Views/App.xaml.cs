﻿using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Samples.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace Samples
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			Instance = this;

			Startup = new Startup();
			Startup.PreInitialize();

			this.InitializeComponent();
		}

		public static Startup Startup { get; private set; }

		public Activity ShellActivity { get; } = new Activity(nameof(Shell));

		public static App Instance { get; private set; }

		public Shell Shell { get; private set; }

		public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

		public Window CurrentWindow { get; private set; }

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			InitializeAndStart(args);
		}

		private void InitializeAndStart(LaunchActivatedEventArgs args)
		{
			//-:cnd:noEmit
#if __WINDOWS__
			CurrentWindow = new Window();
			CurrentWindow.Activate();
#else
			CurrentWindow = Microsoft.UI.Xaml.Window.Current;
#endif
			//+:cnd:noEmit

			Shell = CurrentWindow.Content as Shell;

			var isFirstLaunch = Shell == null;

			if (isFirstLaunch)
			{
				ConfigureViewSize();

				Startup.Initialize();

				ShellActivity.Start();

				CurrentWindow.Content = Shell = new Shell(args);

				ShellActivity.Stop();
			}

			CurrentWindow.Activate();

			_ = Task.Run(() => Startup.Start());
		}

		private void ConfigureViewSize()
		{
#if WINDOWS_UWP
            ApplicationView.PreferredLaunchViewSize = new Size(480, 800);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 480));
#endif
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			// TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		/// <summary>
		/// Configures global Uno Platform logging
		/// </summary>
		private static void InitializeLogging()
		{
			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
				builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif WINDOWS_UWP
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// Generic Xaml events
				// builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

				// Layouter specific messages
				// builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// RemoteControl and HotReload related
				// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Debug JS interop
				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
			});

			//global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

			//#if HAS_UNO
			//			global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
			//#endif
		}
	}
}
