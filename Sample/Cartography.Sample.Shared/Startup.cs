using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Sample.Presentation;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Uno.UI;
using Windows.UI.Core;

namespace Cartography.Sample.Views
{
	public sealed class Startup : StartupBase
	{
		public Startup()
			: base(new CoreStartup())
		{
		}

		protected override void PreInitializeServices()
		{
#if __ANDROID__ || __IOS__

			FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
		}
        protected override void OnInitialized(IServiceProvider services)
        {
        }

		protected override void InitializeViewServices(IHostBuilder hostBuilder)
		{
			hostBuilder.AddViewServices();
		}

		protected override async Task StartViewServices(IServiceProvider services, bool isFirstStart)
		{
			if (isFirstStart)
			{
				// Start your view services here.
				await SetShellViewModel();
			}
		}

		private static async Task SetShellViewModel()
		{
			await App.Instance.Shell.Dispatcher.RunAsync((CoreDispatcherPriority)CoreDispatcherPriority.Normal, SetDataContextUI);

			void SetDataContextUI() // Runs on UI thread
			{
				var shellViewModel = new ShellViewModel();
				shellViewModel.AttachToView(App.Instance.Shell);
				App.Instance.Shell.DataContext = shellViewModel;
			}
		}

    }
}
