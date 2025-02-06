﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Uno.Disposables;

namespace Samples
{
	/// <summary>
	/// This class abstracts the core startup of the app.
	/// This abstract class is responsible for building the host of the application as well as startup diagnostics.
	/// The implementator class is responsible for the application-specific code that initializes the application's services.
	/// </summary>
	public abstract class CoreStartupBase
	{
		/// <summary>
		/// All subscriptions which will never be disposed.
		/// </summary>
		protected static readonly CompositeDisposable _neverDisposed = new CompositeDisposable();

		public StartupState State { get; } = new StartupState();

		public IServiceProvider ServiceProvider { get; private set; }

		public Activity BuildCoreHostActivity { get; } = new Activity("BuildCoreHost");

		public Activity BuildHostActivity { get; } = new Activity("BuildHost");

		protected ILogger Logger { get; private set; }

		/// <summary>
		/// Initializes the application.
		/// </summary>
		/// <param name="extraHostConfiguration">Extra host configuration</param>
		public void Initialize(Action<IHostBuilder> extraHostConfiguration = null)
		{
			if (State.IsInitialized)
			{
				throw new InvalidOperationException($"You shouldn't call {nameof(Initialize)} more than once.");
			}

			var contentRootPath = GetContentRootPath();

			BuildHostActivity.Start();

			var hostBuilder = InitializeServices(new HostBuilder()
				.UseContentRoot(contentRootPath)
			);

			extraHostConfiguration?.Invoke(hostBuilder);

			var host = hostBuilder.Build();

			BuildHostActivity.Stop();

			ServiceProvider = host.Services;

			OnInitialized(ServiceProvider);

			State.IsInitialized = true;
		}

		/// <summary>
		/// Initializes services into the provided <see cref="IHostBuilder"/>.
		/// </summary>
		/// <param name="hostBuilder">The hostbuilder in which services must be added.</param>
		/// <returns>The original host builder with the new services added.</returns>
		protected abstract IHostBuilder InitializeServices(IHostBuilder hostBuilder);

		/// <summary>
		/// This method will be called once the app is initialized.
		/// This is a chance to apply any configuration required to start the app.
		/// </summary>
		/// <param name="services">Services</param>
		protected abstract void OnInitialized(IServiceProvider services);

		/// <summary>
		/// Starts the application.
		/// This method can be called multiple times.
		/// </summary>
		/// <returns><see cref="Task"/></returns>
		public async Task Start()
		{
			if (!State.IsInitialized)
			{
				throw new InvalidOperationException($"You must call {nameof(Initialize)} before calling '{nameof(Start)}'.");
			}

			var isFirstStart = !State.IsStarted;

			await StartServices(ServiceProvider, isFirstStart);

			if (isFirstStart)
			{
				State.IsStarted = true;
			}
		}

		/// <summary>
		/// Starts the services.
		/// This method can be called multiple times.
		/// This method will run on a background thread.
		/// </summary>
		/// <param name="services">Services</param>
		/// <param name="isFirstStart">True if it's the first start; false otherwise.</param>
		/// <returns>Task that completes when the services are started.</returns>
		protected abstract Task StartServices(IServiceProvider services, bool isFirstStart);

		private string GetContentRootPath()
		{
#if WINDOWS_UWP || __ANDROID__ || __IOS__ || __WASM__
			return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
			return string.Empty;
#endif
		}
	}
}
