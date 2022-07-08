using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using nVentive.Umbrella.Client.Commands;
using nVentive.Umbrella.Presentation.Light;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	public class PageViewModel : ViewModelBase
	{
		private readonly Frame _frame;

		public PageViewModel()
		{
			_frame = Windows.UI.Xaml.Window.Current.Content as Frame;

			Build(b => b
				.Properties(pb => pb
					.AttachCommand("NavigateBack", cb => cb.Execute(NavigateBack))
				)
			);
		}

		protected async Task NavigateBack(CancellationToken ct)
		{
			var dispatcher = await this.GetDispatcher(ct);

			await dispatcher.Run(() => _frame.GoBack(), ct);
		}

		protected async Task NavigateTo(CancellationToken ct, Type pageType)
		{
			var dispatcher = await this.GetDispatcher(ct);

			await dispatcher.Run(() => _frame.Navigate(pageType), ct);
		}
	}
}
