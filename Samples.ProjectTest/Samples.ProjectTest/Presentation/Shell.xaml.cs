using Uno.Toolkit.UI;
using Chinook.SectionsNavigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace Samples.ProjectTest.Presentation
{
    public sealed partial class Shell : UserControl, IContentControlProvider
    {
        public Shell()
        {
            this.InitializeComponent();

            Instance = this;
        }

        public ContentControl ContentControl => Splash;

        public static Shell Instance { get; private set; }

        public MultiFrame NavigationMultiFrame => this.RootNavigationMultiFrame;
    }
}