using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Sample.Views.Content;

public sealed partial class SettingsPage : Page
{
	public SettingsPage()
	{
		this.InitializeComponent();
	}

	private void OnThemeButtonClicked(object sender, RoutedEventArgs e)
	{
		// Set theme for window root.
		if (App.Instance.CurrentWindow.Content is FrameworkElement root)
		{
			switch (root.ActualTheme)
			{
				case ElementTheme.Default:
				case ElementTheme.Light:
					root.RequestedTheme = ElementTheme.Dark;
#if __ANDROID__ || __IOS__
					// No need for the dispatcher here.
					Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.Black;
#endif
					break;
				case ElementTheme.Dark:
					root.RequestedTheme = ElementTheme.Light;
#if __ANDROID__ || __IOS__
					// No need for the dispatcher here.
					Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.White;
#endif
					break;
			}
		}
	}
}
