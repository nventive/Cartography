using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Sample.Presentation;
using Chinook.BackButtonManager;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.UI.Core;
using Cartography.StaticMap;

namespace Sample.Views;

public sealed class Startup : StartupBase
{
	public Startup()
		: base(new CoreStartup())
	{
	}

	protected override void PreInitializeServices()
	{
		LocalizationConfiguration.PreInitialize();

#if __ANDROID__ || __IOS__
		Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif
	}

	protected override void InitializeViewServices(IHostBuilder hostBuilder)
	{
		// TODO: Configure your platform-specific service implementations from here.

		hostBuilder.ConfigureServices(s => s
			.AddSingleton<StartupBase>(this)
			.AddLocalization()
			.AddNavigation()
			.AddViewServices()
			.AddApi()
		);
	}

	protected override void OnInitialized(IServiceProvider services)
	{
		// Configures a default refresh command for all DataLoaderView controls.
		DataLoaderView.DefaultRefreshCommandProvider = GetDataLoaderViewRefreshCommand;

		HandleUnhandledExceptions(services);

		ICommand GetDataLoaderViewRefreshCommand(DataLoaderView dataLoaderView)
		{
			return services
				.GetRequiredService<IDynamicCommandBuilderFactory>()
				.CreateFromTask(
					name: "DataLoaderViewRefreshCommand",
					execute: async (ct) =>
					{
						var context = new DataLoaderContext();

						context["IsForceRefreshing"] = true;

						await dataLoaderView.DataLoader.Load(ct, context);
					},
					viewModel: (IViewModel)App.Instance.Shell.DataContext)
				.Build();
		}
	}

	protected override async Task StartViewServices(IServiceProvider services, bool isFirstStart)
	{
		if (isFirstStart)
		{
			// TODO: Start your platform-specific services from here.

			await SetShellViewModel();

			await AddSystemBackButtonSource(services);
			await AddMouseBackButtonSource(services);

			HandleSystemBackVisibility(services);

#if WINDOWS
			StaticMapInitializer.Initialize(services.GetRequiredService<IDispatcherScheduler>(), Constants.BingMaps.ApiKey);
#elif __ANDROID__ || __IOS__
        StaticMapInitializer.Initialize(services.GetRequiredService<IDispatcherScheduler>(), string.Empty);
#endif
		}
	}

	protected override ILogger GetOrCreateLogger(IServiceProvider serviceProvider)
	{
		return serviceProvider.GetRequiredService<ILogger<Startup>>();
	}

	private static void HandleUnhandledExceptions(IServiceProvider services)
	{
		void OnError(Exception e, bool isTerminating = false) => ErrorConfiguration.OnUnhandledException(e, isTerminating, services);

#if __WINDOWS__ || __ANDROID__ || __IOS__

		Application.Current.UnhandledException += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
#endif
#if __ANDROID__
		Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
		{
			OnError(e.Exception);
			e.Handled = true;
		};
#endif
	}

	private static async Task SetShellViewModel()
	{
		await App.Instance.Shell.DispatcherQueue.EnqueueAsync(SetDataContextUI);

		static void SetDataContextUI() // Runs on UI thread.
		{
			var shellViewModel = new ShellViewModel();

			shellViewModel.AttachToView(App.Instance.Shell);

			App.Instance.Shell.DataContext = shellViewModel;
		}
	}

	/// <summary>
	/// Sets the visibility of the system UI's back button based on the navigation controller.
	/// </summary>
	private void HandleSystemBackVisibility(IServiceProvider services)
	{
#if __ANDROID__ || __IOS__
		var multiNavigationController = services.GetRequiredService<ISectionsNavigator>();

		Observable
			.FromEventPattern<SectionsNavigatorStateChangedEventHandler, SectionsNavigatorEventArgs>(
				h => multiNavigationController.StateChanged += h,
				h => multiNavigationController.StateChanged -= h
			)
			.Select(_ => multiNavigationController.CanNavigateBackOrCloseModal())
			.StartWith(multiNavigationController.CanNavigateBackOrCloseModal())
			.DistinctUntilChanged()
			.Subscribe(OnStateChanged);

		void OnStateChanged(bool canNavigateBackOrCloseModal)
		{
			var dispatcherQueue = services.GetRequiredService<DispatcherQueue>();
			_ = dispatcherQueue.EnqueueAsync(UpdateBackButtonUI);

			void UpdateBackButtonUI() // Runs on UI thread.
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = canNavigateBackOrCloseModal
					? AppViewBackButtonVisibility.Visible
					: AppViewBackButtonVisibility.Collapsed;
			}
		}
#endif
	}

	/// <summary>
	/// Adds SystemNavigation's back requests to the IBackButtonManager. Those requests are usually sent via the top bars back buttons.
	/// </summary>
	private async Task AddSystemBackButtonSource(IServiceProvider services)
	{
#if __ANDROID__ || __IOS__
		var dispatcherQueue = services.GetRequiredService<DispatcherQueue>();
		var backButtonManager = services.GetRequiredService<IBackButtonManager>();
		await dispatcherQueue.EnqueueAsync(AddSystemBackButtonSourceInner, DispatcherQueuePriority.High);

		// Runs on main thread.
		void AddSystemBackButtonSourceInner()
		{
			var source = new SystemNavigationBackButtonSource();
			backButtonManager.AddSource(source);
		}
#endif
	}

	/// <summary>
	/// Adds mouse back button to the IBackButtonManager.
	/// </summary>
	private async Task AddMouseBackButtonSource(IServiceProvider services)
	{
#if WINDOWS
		var dispatcherQueue = services.GetRequiredService<DispatcherQueue>();
		var backButtonManager = services.GetRequiredService<IBackButtonManager>();
		await dispatcherQueue.EnqueueAsync(AddSystemBackButtonSourceInner, DispatcherQueuePriority.High);

		// Runs on main thread.
		void AddSystemBackButtonSourceInner()
		{
			var source = new PointerXButton1BackButtonSource(App.Instance.Shell);
			backButtonManager.AddSource(source);
		}
#endif
	}

}
