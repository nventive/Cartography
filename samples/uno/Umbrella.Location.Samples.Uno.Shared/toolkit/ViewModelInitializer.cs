using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nVentive.Umbrella.Concurrency;
using nVentive.Umbrella.Presentation.Light;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	public static class ViewModelInitializer
	{
		public static void InitializeViewModel(FrameworkElement viewToBind, Func<ViewModelBase> viewModelFactory)
		{
			var viewModel = viewModelFactory();

			viewModel.ServiceLocator = CommonServiceLocator.ServiceLocator.Current;

			Schedulers
				.Default
				.ScheduleTask((ct2, _) => BindViewToViewModel(ct2, viewToBind, viewModel))
				.DisposeWith(viewModel.Subscriptions);
		}

		private static async Task BindViewToViewModel(CancellationToken ct, FrameworkElement viewToBind, ViewModelBase viewModel)
		{
			try
			{
				var dispatcher = new CoreDispatcherScheduler(viewToBind.Dispatcher, Windows.UI.Core.CoreDispatcherPriority.High);

				viewModel.SetView(new GenericView(viewToBind, dispatcher, alreadyLoaded: true));

				await dispatcher.Run(() => viewToBind.DataContext = viewModel, ct);
			}
			catch (OperationCanceledException)
			{
				// No worries if the operation has been canceled, probably an early back navigation.
			}
		}
	}
}
