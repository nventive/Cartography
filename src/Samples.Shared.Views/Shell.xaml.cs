using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Samples
{
	public sealed partial class Shell : UserControl
	{
		public Shell(LaunchActivatedEventArgs e)
		{
			this.InitializeComponent();

			Instance = this;

		}

		public static Shell Instance { get; private set; }

		public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
	}
}
