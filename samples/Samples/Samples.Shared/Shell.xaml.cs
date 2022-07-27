using Chinook.SectionsNavigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

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
