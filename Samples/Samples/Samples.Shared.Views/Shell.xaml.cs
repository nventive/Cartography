using Chinook.SectionsNavigation;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;

namespace Samples
{
	public sealed partial class Shell : UserControl
	{
		public Shell(IActivatedEventArgs e)
		{
			this.InitializeComponent();

			Instance = this;

		}

		public static Shell Instance { get; private set; }

		public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
	}
}
