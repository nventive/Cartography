using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using nVentive.Umbrella.Client.Commands;
using nVentive.Umbrella.Presentation.Light;
using Umbrella.Location.Core;
using Umbrella.Location.Samples.Uno.Shared;
using Umbrella.Location.StaticMap;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Umbrella.Location.Samples.Uno
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StaticMapPage : Page
	{
		public StaticMapPage()
		{
			var providers = GetStaticMapProviders();

			StaticMapControl.StaticMapProvider = providers.FirstOrDefault();

			this.InitializeComponent();

			ViewModelInitializer.InitializeViewModel(this, () => new StaticMapPageViewModel(providers));
		}

		private IStaticMapProvider[] GetStaticMapProviders()
		{
#if NETFX_CORE
			return new IStaticMapProvider[]
			{
				new BingStaticMapProvider(Constants.BingMaps.ApiKey),
				new GoogleStaticMapProvider(Constants.GoogleMaps.ApiKey, Constants.GoogleMaps.Secret)
			};
#elif __ANDROID__
			return new IStaticMapProvider[] { new GoogleSdkStaticMapProvider() };
#elif __IOS__
			Google.Maps.MapServices.ProvideApiKey(Constants.GoogleMaps.ApiKey);

			return new IStaticMapProvider[]
			{
				new AppleStaticMapProvider(),
				new GoogleSdkStaticMapProvider()
			};
#endif
		}

		public class StaticMapPageViewModel : PageViewModel
		{
			public StaticMapPageViewModel(IStaticMapProvider[] providers)
			{
				Providers = providers;
				HasMultipleProviders = providers.Count() > 1;

				Build(b => b
					.Properties(pb => pb
						.Attach(Latitude, () => "45.5016889")
						.Attach(Longitude, () => "-73.56725599999999")
						.Attach(ZoomLevel, () => 12.0)
						.Attach(Height, () => "300")
						.Attach(Width, () => "300")
						.Attach(MapViewPort, GetMapViewPort)
						.Attach(MapSize, () => new Size(300, 300))
						.Attach(ProviderIndex, () => 0)
						.AttachCommand("ShowStaticMap", cb => cb.Execute(ShowStaticMap))
					)
				);
			}

			public IStaticMapProvider[] Providers { get; }

			public bool HasMultipleProviders { get; }

			private IDynamicProperty<string> Latitude => this.GetProperty<string>();

			private IDynamicProperty<string> Longitude => this.GetProperty<string>();

			private IDynamicProperty<MapViewPort> MapViewPort => this.GetProperty<MapViewPort>();

			private IDynamicProperty<double> ZoomLevel => this.GetProperty<double>();

			private IDynamicProperty<Size> MapSize => this.GetProperty<Size>();

			private IDynamicProperty<string> Height => this.GetProperty<string>();

			private IDynamicProperty<string> Width => this.GetProperty<string>();

			private IDynamicProperty<int> ProviderIndex => this.GetProperty<int>();

			private int CurrentIndex { get; set; } = 0;

			private MapViewPort GetMapViewPort()
			{
				return new MapViewPort(new Coordinate()
				{
					Latitude = 45.5016889,
					Longitude = -73.56725599999999
				})
				{
					ZoomLevel = ZoomLevels.City
				};
			}

			private async Task ShowStaticMap(CancellationToken ct)
			{
				double latitude = double.Parse(await Latitude, NumberStyles.Any, CultureInfo.InvariantCulture);
				double longitude = double.Parse(await Longitude, NumberStyles.Any, CultureInfo.InvariantCulture);
				double height = double.Parse(await Height, NumberStyles.Any, CultureInfo.InvariantCulture);
				double width = double.Parse(await Width, NumberStyles.Any, CultureInfo.InvariantCulture);
				double zoomLevel = await ZoomLevel;
				int index = await ProviderIndex;

				if (index != CurrentIndex)
				{
					StaticMapControl.StaticMapProvider = Providers[index];

					CurrentIndex = index;

					// Note: This is needed to test different providers easily.
					// If MapViewPort does not change at all, the StaticMapControl won't update
					latitude += 0.00000000001;
				}

				MapViewPort mapViewPort = new MapViewPort(new Coordinate()
				{
					Latitude = latitude,
					Longitude = longitude
				})
				{
					ZoomLevel = new ZoomLevel(Math.Round(zoomLevel))
				};

				MapSize.Value.OnNext(new Size(width, height));

				MapViewPort.Value.OnNext(mapViewPort);
			}
		}
	}
}
