using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Samples.Presentation;
using Windows.UI.Core;
using Samples.Helpers;

namespace Samples
{
	public sealed class CoreStartup : CoreStartupBase
	{
		protected override IHostBuilder InitializeServices(IHostBuilder hostBuilder)
		{
			return hostBuilder
				.AddAppSettings()
				.AddServices();
		}

		protected override void OnInitialized(IServiceProvider services)
		{
			ViewModelBase.DefaultServiceProvider = services;
		}

		protected override async Task StartServices(IServiceProvider services, bool isFirstStart)
		{
			if (isFirstStart)
			{
				// Start your services here.

				await ExecuteInitialNavigation(CancellationToken.None, services);
			}
		}

		private async Task ExecuteInitialNavigation(CancellationToken ct, IServiceProvider services)
		{
			var sectionsNavigator = services.GetRequiredService<ISectionsNavigator>();

			await sectionsNavigator.SetActiveSection(ct, "Main");

			await sectionsNavigator.NavigateAndClear(ct, () => new MainPageViewModel());
		}
	}
}
