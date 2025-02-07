using System.Threading.Tasks;
using Sample.Views;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Uno.Extensions;
using Uno.UI;
using Windows.Graphics.Display;

namespace Sample;

public sealed partial class App : Application
{
	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		InitializeComponent();

		ConfigureOrientation();

#if __MOBILE__
		LeavingBackground += OnLeavingBackground;
		Resuming += OnResuming;
		Suspending += OnSuspending;
#endif
	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void InitializeAndStart()
	{
#if __WINDOWS__
		CurrentWindow = new Window();
		CurrentWindow.Activate();
#else
		CurrentWindow = Microsoft.UI.Xaml.Window.Current;
#endif

		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureWindow();
			ConfigureStatusBar();

			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			CurrentWindow.Content = Shell = new Shell();

			Startup.ShellActivity.Stop();
		}

#if __MOBILE__
		CurrentWindow.Activate();
#endif

#if DEBUG
		CurrentWindow.EnableHotReload();
#endif

		_ = Task.Run(() => Startup.Start());
	}

#if __MOBILE__
	/// <summary>
	/// This is where your app launches if you use custom schemes, Universal Links, or Android App Links.
	/// </summary>
	/// <param name="args"><see cref="Windows.ApplicationModel.Activation.IActivatedEventArgs"/>.</param>
	protected override void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void OnLeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
	{
		this.Log().LogInformation("Application is leaving background.");
	}

	private void OnResuming(object sender, object e)
	{
		this.Log().LogInformation("Application is resuming.");
	}

	private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
	{
		this.Log().LogInformation("Application is suspending.");
	}
#endif

	private static string GetContentRootPath()
	{
#if __WINDOWS__ || __MOBILE__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
	}

	private static string GetSettingsFolderPath()
	{
#if __WINDOWS__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#endif
	}

	private static void ConfigureOrientation()
	{
		DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
	}

	private static void ConfigureStatusBar()
	{
		var resources = Current.Resources;
		var statusBarHeight = 0d;

#if __MOBILE__
		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.White;
		statusBarHeight = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
#endif

		resources.Add("StatusBarDouble", statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}

	private void ConfigureWindow()
	{
#if __WINDOWS__
		var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);
		var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
		var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
		appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 480, Height = 800 });

		// Sets a title bar icon and title.
		// Workaround. See https://github.com/microsoft/microsoft-ui-xaml/issues/6773 for more details.
		appWindow.SetIcon("Images\\TitleBarIcon.ico");
		appWindow.Title = "Sample";
#endif
	}
}
