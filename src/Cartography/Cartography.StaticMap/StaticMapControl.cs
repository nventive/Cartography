#if WINDOWS_UWP || __IOS__ || __ANDROID__
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Uno.Disposables;
using SerialDisposable = Uno.Disposables.SerialDisposable;
using CompositeDisposable = Uno.Disposables.CompositeDisposable;
using Uno.Extensions;
using Uno.Logging;
using Cartography.StaticMap.Provider;
#if __IOS__
using UIKit;
using Uno.UI;
#elif __ANDROID__
using Uno.UI;
#elif WINDOWS_UWP
using System.Windows;
using Windows.Graphics.Display;
#endif

namespace Cartography.StaticMap
{
	/// <summary>
	/// Displays a static image of a map retrieved from a StaticMapProvider.
	/// </summary>
	public partial class StaticMapControl
	{
		#region DependencyProperties

		/// <summary>
		/// Dependency property to the MapViewPort.
		/// </summary>
		public static readonly DependencyProperty MapViewPortProperty =
			DependencyProperty.Register("MapViewPort", typeof(StaticMapViewPort), typeof(StaticMapControl), new PropertyMetadata(default(StaticMapViewPort), ParameterChanged));

		/// <summary>
		/// Dependency property to the MapSize.
		/// </summary>
		public static readonly DependencyProperty MapSizeProperty =
			DependencyProperty.Register("MapSize", typeof(Size), typeof(StaticMapControl), new PropertyMetadata(default(Size), ParameterChanged));

		/// <summary>
		/// Dependency property to the PushPin
		/// </summary>
		public static readonly DependencyProperty PushpinProperty =
			DependencyProperty.Register("Pushpin", typeof(object), typeof(StaticMapControl), new PropertyMetadata(default(object)));

		/// <summary>
		/// Dependency property to the PushPinTemplate
		/// </summary>
		public static readonly DependencyProperty PushpinTemplateProperty =
			DependencyProperty.Register("PushpinTemplate", typeof(DataTemplate), typeof(StaticMapControl), new PropertyMetadata(default(DataTemplate)));

		/// <summary>
		/// Dependency property to the PushpinTemplate Selector
		/// </summary>
		public static readonly DependencyProperty PushpinTemplateSelectorProperty =
			DependencyProperty.Register("PushpinTemplateSelector", typeof(DataTemplateSelector), typeof(StaticMapControl), new PropertyMetadata(default(DataTemplateSelector)));
		#endregion

		private const string DefaultState = "Default";
		private const string LoadingState = "Loading";
		private const string LoadedState = "Loaded";
		private const string ErrorState = "Error";

		private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
		private readonly SerialDisposable _mapRequest;
		private readonly Subject<Unit> _updateSubject = new Subject<Unit>();
		private static readonly TimeSpan UpdateThrottle = TimeSpan.FromMilliseconds(250);

		private bool _isLoadingNewMap = false;
		private bool _viewReady;
		private StaticMapParameters _lastUsedParameters;

		/// <summary>
		/// Create a new instance of the <see cref="StaticMapControl"/> class. For initializing the StaticMapControl <see cref="StaticMapInitializer"/>.
		/// </summary>
		public StaticMapControl()
		{
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;

			InnerConstruct();

			_mapRequest = new SerialDisposable().DisposeWith(_subscriptions);

			SubscribeToUpdate();

		}

		/// <summary>
		/// Gets or sets the viewport of the map.
		/// </summary>
		public StaticMapViewPort MapViewPort
		{
			get { return (StaticMapViewPort)this.GetValue(MapViewPortProperty); }
			set { this.SetValue(MapViewPortProperty, value); }
		}

		/// <summary>
		/// Gets or sets the size of the static map
		/// If the MapSize is null, the static map's size will be equal to the actual StaticMapControl size.
		/// If the MapSize is not null, the static map's size will be equal to the MapSize.
		/// Note that the MapSize is subject to size constraints from the StaticMapProvider.
		/// </summary>
		public Size MapSize
		{
			get { return (Size)this.GetValue(MapSizeProperty); }
			set { this.SetValue(MapSizeProperty, value); }
		}

		/// <summary>
		/// Gets or sets the Pushpin
		/// </summary>
		public object Pushpin
		{
			get { return (object)GetValue(PushpinProperty); }
			set { SetValue(PushpinProperty, value); }
		}

		/// <summary>
		/// Gets or sets the PushPin Template
		/// </summary>
		public DataTemplate PushpinTemplate
		{
			get { return (DataTemplate)GetValue(PushpinTemplateProperty); }
			set { SetValue(PushpinTemplateProperty, value); }
		}

		/// <summary>
		/// Gets or sets the push pin template selector
		/// </summary>
		public DataTemplateSelector PushpinTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(PushpinTemplateSelectorProperty); }
			set { SetValue(PushpinTemplateSelectorProperty, value); }
		}

		/// <summary>
		/// Gets map size to display
		/// In order to prevent flicks on device rotation, we need to load a larger image in order to avoid reload every time the device is rotated
		/// </summary>
		/// <returns>Returns max displayed map size</returns>
		public Size GetMaxMapSize()
		{
			if (UseFixedMapSize())
			{
				return MapSize;
			}

			// We need to measure the higher value on both landscape and portrait to load image only once if nothing more than the displayed size changes
			var bounds = Windows.UI.Xaml.Window.Current.Bounds;
			var maxBoundsMeasure = Math.Max(bounds.Height, bounds.Width);

			return new Size(
				double.IsNaN(Width) || double.IsInfinity(Width) ? maxBoundsMeasure : Width,
				double.IsNaN(Height) || double.IsInfinity(Height) ? maxBoundsMeasure : Height
			);
		}

		private static void ParameterChanged(object dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			var staticMapControl = dependencyObject as StaticMapControl;
			staticMapControl?.RequestUpdate();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_mapRequest.Disposable = null;
			_lastUsedParameters = null;
		}

		partial void InnerConstruct();

		private void SubscribeToUpdate()
		{
			_updateSubject
				.Throttle(UpdateThrottle, StaticMapInitializer.DispatcherScheduler)
				.Do(_ => Update())
				.Subscribe(_ => { }, e => this.Log().Error("Error in Update subscription of StaticMapControl"))
				.DisposeWith(_subscriptions);
		}

		private void GoToState(string state)
		{
			VisualStateManager.GoToState(this, state, false);
		}

		private void RequestUpdate()
		{
			// The first valid request needs to be executed without throttling.
			// Otherwise, a blank map will be displayed until the last request is done.
			// We don't want a MinDelaySample, as it will display
			// a blank map on subsequent requests (after the delay).
			if (!_isLoadingNewMap)
			{
				Update();
			}

			// Every following requests need to be throttled
			else
			{
				_updateSubject.OnNext(new Unit());
			}
		}

		private void Update()
		{
			if (!_viewReady)
			{
				return;
			}

			if (MapViewPort == null)
			{
				SetMap(null);
				return;
			}

			var p = GetViewPortParams();

			// Don't execute a new query for the same parameters
			if (p.Equals(_lastUsedParameters))
			{
				return;
			}

			// TODO The StaticMapProvider.GetMap should not have to be executed on the DispatcherScheduler.
			// Right now it's not a big deal since it's just a url that is created.
			_mapRequest.Disposable = StaticMapInitializer.DispatcherScheduler
				.ScheduleAsync(
					async (action, c) =>
					{
						await GetMap(c, p);
					}
				);
		}

		private StaticMapParameters GetViewPortParams()
		{
			var mapSize = GetMaxMapSize();

			return new StaticMapParameters()
			{
#if __ANDROID__
						Scale = ViewHelper.Scale,
#elif __IOS__
				Scale = ViewHelper.MainScreenScale,
#elif WINDOWS_UWP
						Scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel,
#endif
				Width = (int)mapSize.Width,
				Height = (int)mapSize.Height,
				ViewPort = MapViewPort
			};
		}

		private async Task GetMap(CancellationToken ct, StaticMapParameters parameters)
		{
			try
			{
				_isLoadingNewMap = true;

				GoToState(LoadingState);

				if (parameters == null)
				{
					SetMap(null);
				}
				else
				{
					bool viewportPropChanged = _lastUsedParameters == null || (parameters.Scale != _lastUsedParameters.Scale || parameters.ViewPort != _lastUsedParameters.ViewPort);

					if (viewportPropChanged)
					{
						var map = await StaticMapInitializer.StaticMapProvider.GetMap(ct, parameters);
						SetMap(map, parameters);
					}
				}

				_lastUsedParameters = parameters;

				// This need to be called in order to update the visible part of the map image when the device is rotated
				UpdateLayout();

				GoToState(LoadedState);
			}
			catch (Exception e)
			{

				GoToState(ErrorState);

				_lastUsedParameters = null;
			}
			finally
			{
				_isLoadingNewMap = false;
			}
		}

		private bool UseFixedMapSize()
		{
			return IsValidSize(MapSize);
		}

		private bool IsValidSize(Size size)
		{
			return size.Width > 0 && size.Height > 0;
		}
	}
}
#endif
