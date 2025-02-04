#if __MOBILE__ //|| WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Threading;
using Uno.Extensions;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using GeolocatorService;
using Microsoft.Extensions.DependencyInjection;
using Uno;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#if WINDOWS
using Windows.Foundation;
using Map = Microsoft.UI.Xaml.Controls.MapControl;
#else
using System.Drawing;
#endif

namespace Cartography.DynamicMap;

/// <summary>
/// The control which display the map component configured on <see cref="ViewModel"/>.
/// </summary>
#if WINDOWS
[TemplatePart(Name = _mapPartName, Type = typeof(Map))]
[TemplatePart(Name = _errorPresenterPartName, Type = typeof(ContentPresenter))]
[TemplateVisualState(GroupName = "ControlStates", Name = _initializingStateName)]
[TemplateVisualState(GroupName = "ControlStates", Name = _readyStateName)]
[TemplateVisualState(GroupName = "ControlStates", Name = _errorStateName)]
#endif
public abstract partial class MapControlBase : Control
{
    private const string _mapPartName = "PART_map";
    private const string _errorPresenterPartName = "PART_ErrorPresenter";

    private const string _initializingStateName = "Initializing";
    private const string _readyStateName = "Ready";
    private const string _errorStateName = "Error";
    private ILogger<MapControlBase> _logger;

    #region ViewModel (dp)
    /// <summary>
    /// Identifies the <see cref="ViewModel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        "ViewModel", typeof(object), typeof(MapControlBase), new PropertyMetadata(default(object), OnViewModelChanged));

    private static void OnViewModelChanged(object d, DependencyPropertyChangedEventArgs e)
    {
        (d as MapControlBase).Maybe(ctl => ctl.TryStart(e.NewValue as ViewModelBase));
    }

    /// <summary>
    /// Gets or sets the <see cref="ViewModelBase"/> on which the map component was configured.
    /// </summary>
    public ViewModelBase ViewModel
    {
        get { return (ViewModelBase)this.GetValue(ViewModelProperty); }
        set { this.SetValue(ViewModelProperty, value); }
    }
    #endregion

    #region SelectionMode (dp)
    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
        "SelectionMode", typeof(MapSelectionMode), typeof(MapControlBase), new PropertyMetadata(MapSelectionMode.Single));

    /// <summary>
    /// Gets or sets the selection mode of pushins on the map.
    /// </summary>
    public MapSelectionMode SelectionMode
    {
        get { return (MapSelectionMode)this.GetValue(SelectionModeProperty); }
        set { this.SetValue(SelectionModeProperty, value); }
    }
    #endregion

    #region AutolocateButtonVisibility (dp)
    public Visibility AutolocateButtonVisibility
    {
        get { return (Visibility)GetValue(AutolocateButtonVisibilityProperty); }
        set { SetValue(AutolocateButtonVisibilityProperty, value); }
    }

    /// <summary>
    /// Gets or sets the visibility of the button which 
    /// centers the viewport on the current location of the device.
    /// </summary>
    public static readonly DependencyProperty AutolocateButtonVisibilityProperty =
        DependencyProperty.Register("AutolocateButtonVisibility", typeof(Visibility), typeof(MapControlBase), new PropertyMetadata(Visibility.Visible, (s, e) => ((MapControlBase)s)?.OnAutolocateButtonVisibilityChanged(e)));

    private void OnAutolocateButtonVisibilityChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateAutolocateButtonVisibility((Visibility)e.NewValue);
    }

    partial void UpdateAutolocateButtonVisibility(Visibility visibility);
    #endregion

    #region CompassButtonVisibility (dp)
    public Visibility CompassButtonVisibility
    {
        get { return (Visibility)GetValue(CompassButtonVisibilityProperty); }
        set { SetValue(CompassButtonVisibilityProperty, value); }
    }

    /// <summary>
    /// Gets or sets the visibility of the Compass button once the viewport
    /// is rotated. The compass resets the rotataion.
    /// </summary>
    public static readonly DependencyProperty CompassButtonVisibilityProperty =
        DependencyProperty.Register("CompassButtonVisibility", typeof(Visibility), typeof(MapControlBase), new PropertyMetadata(Visibility.Visible, (s, e) => ((MapControlBase)s)?.CompassButtonVisibilityChanged(e)));

    private void CompassButtonVisibilityChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateCompassButtonVisibility((Visibility)e.NewValue);
    }

    partial void UpdateCompassButtonVisibility(Visibility visibility);
    #endregion

    #region MapStyleJson (dp)
    public string MapStyleJson
    {
        get { return (string)GetValue(MapStyleJsonProperty); }
        set { SetValue(MapStyleJsonProperty, value); }
    }

    /// <summary>
    /// Gets or sets the visibility of points of interest on the map
    /// </summary>
    public static readonly DependencyProperty MapStyleJsonProperty =
        DependencyProperty.Register("MapStyleJson", typeof(string), typeof(MapControlBase), new PropertyMetadata(string.Empty, (s, e) => ((MapControlBase)s)?.OnMapStyleJsonChanged(e)));

    private void OnMapStyleJsonChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateMapStyleJson((string)e.NewValue);
    }

    partial void UpdateMapStyleJson(string mapStyleJson);
    #endregion

    #region IsRotateGestureEnabled (dp)
    public bool IsRotateGestureEnabled
    {
        get { return (bool)GetValue(IsRotateGestureEnabledProperty); }
        set { SetValue(IsRotateGestureEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsRotateGestureEnabledProperty =
        DependencyProperty.Register("IsRotateGestureEnabled", typeof(bool), typeof(MapControlBase), new PropertyMetadata(true, (s, e) => ((MapControlBase)s)?.OnIsRotateGestureEnabledChanged(e)));

    private void OnIsRotateGestureEnabledChanged(DependencyPropertyChangedEventArgs e)
    {
        UpdateIsRotateGestureEnabled((bool)e.NewValue);
    }

    partial void UpdateIsRotateGestureEnabled(bool isRotateGestureEnabled);
    #endregion

#if WINDOWS
#region PushpinItemTemplate (dp)
	/// <summary>
	/// Identifies the <see cref="PushpinItemTemplate"/> dependency property.
	/// </summary>
	public static readonly DependencyProperty PushpinItemTemplateProperty = DependencyProperty.Register(
		"PushpinItemTemplate", typeof(DataTemplate), typeof(MapControl), new PropertyMetadata(default(DataTemplate)));

	/// <summary>
	/// Gets or sets the template of pushpin
	/// </summary>
	public DataTemplate PushpinItemTemplate
	{
		get { return (DataTemplate)this.GetValue(PushpinItemTemplateProperty); }
		set { this.SetValue(PushpinItemTemplateProperty, value); }
	}
#endregion
#endif

    #region PushpinIcon (dp)
    public static readonly DependencyProperty PushpinIconProperty = DependencyProperty.Register(
        "PushpinIcon", typeof(object), typeof(MapControlBase), new PropertyMetadata(default(object), OnPushpinItemIconChanged));

    private static void OnPushpinItemIconChanged(object d, DependencyPropertyChangedEventArgs e)
    {
        ((MapControlBase)d).UpdateIcon(e.NewValue);
    }

    public object PushpinIcon
    {
        get { return this.GetValue(PushpinIconProperty); }
        set { this.SetValue(PushpinIconProperty, value); }
    }
    #endregion

    #region SelectedPushpinIcon (dp)
    public static readonly DependencyProperty SelectedPushpinIconProperty = DependencyProperty.Register(
        "SelectedPushpinIcon", typeof(object), typeof(MapControlBase), new PropertyMetadata(default(object), OnSelectedPushpinItemIconChanged));

    private static void OnSelectedPushpinItemIconChanged(object d, DependencyPropertyChangedEventArgs e)
    {
        ((MapControlBase)d).UpdateSelectedIcon(e.NewValue);
    }

    public object SelectedPushpinIcon
    {
        get { return this.GetValue(SelectedPushpinIconProperty); }
        set { this.SetValue(SelectedPushpinIconProperty, value); }
    }
    #endregion

    #region PushpinIconsPositionOrigin (dp)
    public static readonly DependencyProperty PushpinsIconsPositionOriginProperty = DependencyProperty.Register(
        "PushpinIconsPositionOrigin", typeof(Point), typeof(MapControlBase), new PropertyMetadata(new Point(0, 0)));

    public Point PushpinIconsPositionOrigin
    {
        get { return (Point)this.GetValue(PushpinsIconsPositionOriginProperty); }
        set { this.SetValue(PushpinsIconsPositionOriginProperty, value); }
    }
    #endregion

    private readonly SerialDisposable _configuredSourceSubscriptions = new SerialDisposable();
    private ViewModelBase _configuredViewModel;

    private Action<Windows.Devices.Geolocation.Geocoordinate> _onMapTapped;

    private bool _isReady;

    protected IScheduler GetBackgroundScheduler()
    {
        return GetDispatcherScheduler().ToBackgroundScheduler();
    }

    protected IScheduler GetDispatcherScheduler()
    {
        return ViewModel.GetService<IDispatcherScheduler>();
    }

    private void TryStart()
    {
        TryStart(ViewModel);
    }

    /// <summary>
    /// creation of MapControlBase
    /// </summary>
    /// <param name="logger">logger</param>
    public MapControlBase(ILogger<MapControlBase> logger = null)
    {
        this.DefaultStyleKey = typeof(MapControlBase);
        PartialConstructor(logger);
    }

    partial void PartialConstructor(ILogger<MapControlBase> logger = null);

    private void TryStart(ViewModelBase viewModel)
    {
        try
        {
            GoToInitializingState();

            // Do not reinitialize same Source
            if (!_isReady || _configuredViewModel == viewModel)
            {
                return;
            }

            if (!(viewModel is IDynamicMapComponent))
            {
                throw new Exception("The ViewModel must use inheritance of IMapCompoment");
            }

            // Reset pending subscriptions
            _configuredSourceSubscriptions.Disposable = null;
            _configuredViewModel = viewModel;

            if (viewModel != null)
            {
                // This will run when the actual viewmodel is getting dispose. Needed because it created EngineException.
                // it basically clears the points on the map
                viewModel.AddDisposable(new DisposableAction(async () =>
                {
                    await GetDispatcherScheduler().Run(async ct =>
                        {
                            UpdateMapUserLocation(new LocationResult(false, null));
                            UpdateMapPushpins(new IGeoLocated[0], new IGeoLocated[0]);
                        },
                        CancellationToken.None);
                }));

                _configuredSourceSubscriptions.Disposable = new CompositeDisposable(2)
                        {
                            Initialize(viewModel),
                            Disposable.Create(() => Interlocked.CompareExchange(ref _configuredViewModel, null, viewModel)),
                        };
            }
        }
        catch (Exception e)
        {
            GoToErrorState(new MapException(e));
        }
    }

    private void Pause()
    {
        _configuredSourceSubscriptions.Disposable = null;
    }

    private IDisposable Initialize(ViewModelBase viewModel)
    {
        return GetDispatcherScheduler().ScheduleAsync(viewModel, async (_, vm, ct) =>
        {
            var mc = (IDynamicMapComponent)vm;
            _onMapTapped = mc.OnMapTapped;

            SetAnimationDuration(mc.AnimationDurationSeconds);

            return new CompositeDisposable()
                {
                    SyncPushpinsFrom(vm),
                    SyncViewPortTo(vm),
                    SyncViewPortFrom(vm),
                    SyncUserLocationFrom(vm),
                    SyncSelectedPushpinsTo(vm),
                    SyncIsUserDragging(vm),
                };
        });
    }

    protected void OnMapTapped(Windows.Devices.Geolocation.Geocoordinate coordinate)
    {
        var onMapTapped = _onMapTapped;

        if (onMapTapped != null)
        {
            GetDispatcherScheduler()
                .ScheduleAsync(async (ct, scheduler) => onMapTapped(coordinate))
                .Dispose();
        }
    }

    #region Visual states
    private string _visualState = _initializingStateName;
    private MapException _lastError;

    private void ReloadVisualState()
    {
        GetDispatcherScheduler().Schedule(() => GoToState(_visualState));
    }

    private void GoToInitializingState()
    {
        ReloadVisualState();
    }

    private void GoToReadyState()
    {
        Interlocked.CompareExchange(ref _visualState, _readyStateName, _initializingStateName);
        ReloadVisualState();
    }

    private void GoToErrorState(MapException error)
    {
        _lastError = error;
        _visualState = _errorStateName;
        ReloadVisualState();
    }

    private void GoToState(string stateName)
    {
#if HAS_WINDOWS_UI
				if (_isReady)
				{
					_errorPresenter.Content = _lastError;
					VisualStateManager.GoToState(this, stateName, true);
				}
#endif
    }

    #endregion

    #region UserLocation
    private IDisposable SyncUserLocationFrom(ViewModelBase vm)
    {
        var component = (IDynamicMapComponent)vm;
        var userLocation = vm.GetProperty<LocationResult>(nameof(component.UserLocation)).GetAndObserve();

        return userLocation
            .ObserveOn(GetDispatcherScheduler())
            .Do(UpdateMapUserLocation)
            .Subscribe(_ => { }, e => _logger.LogError("SyncUserLocationFrom", e));
    }
    #endregion

    #region ViewPort & ViewPortCoordinates
    private bool _isViewPortInitialized;
    private bool _isAnimating;

    protected bool IsAnimating => _isAnimating;

    private IDisposable SyncViewPortTo(ViewModelBase vm)
    {
        var component = (IDynamicMapComponent)vm;
        //// Do not publish viewPort to VM until it provides it initial position
        var aViewPortWasProvidedBySource = vm.GetProperty<MapViewPort>(nameof(component.ViewPort)).GetAndObserve()
                .Do(_ => _isViewPortInitialized = true)
                .Take(1);

        // View to ViewModel
        return GetViewPortChangedTriggers()
            .Merge()
            .SkipWhile(_ => !GetInitializationStatus())
            .SkipUntil(aViewPortWasProvidedBySource?.Do(_ => GoToReadyState()))
            .MinDelaySample(component.ViewPortUpdateMinDelay, GetBackgroundScheduler()) // Limit the number of Refresh. When not throttle we try to update a dispose object
            .ObserveOn(GetDispatcherScheduler())
            .Scan(
                GetViewPort(),
                (previous, _) =>
                    GetEffectiveViewPort(previous, GetViewPort(), component.ViewPortUpdateFilter))
            .Select(vp =>
                (ViewPort: vp, Coordinates: GetViewPortCoordinates()))
            .ObserveOn(GetBackgroundScheduler())
            .Do(l =>
            {
                _logger.LogDebug("MapControl.SyncViewPortTo({0})", l.ViewPort);
                _logger.LogInformation($"Moved the view port (zoom level: '{l.ViewPort.ZoomLevel}', points of interest: '{l.ViewPort.PointsOfInterest}').");

                if (!_isAnimating)
                {
                    component.IsUserTrackingCurrentlyEnabled = false;
                }
                component.ViewPortCoordinates = l.Coordinates;
                component.ViewPort = l.ViewPort;
            })
            .Subscribe(_ => { }, e => _logger.LogError($"There was an error syncing the view port to '{component}'", e));
    }

    private MapViewPort GetEffectiveViewPort(MapViewPort original, MapViewPort current, IEqualityComparer<MapViewPort> filter)
    {
        if ((filter != null) && filter.Equals(original, current))
        {
            return original;
        }
        else
        {
            return current;
        }
    }

    private IDisposable SyncViewPortFrom(ViewModelBase vm)
    {

        var component = (IDynamicMapComponent)vm;
        var viewPort = vm.GetProperty<MapViewPort>(nameof(component.ViewPort)).GetAndObserve();

        // Source to View
        return viewPort
            .Where(t => t != null)
                .SelectManyDisposePrevious(
                async (viewport, ct) =>
                {
                    await TrySetViewPort(ct, component.ViewPort);
                    component.ViewPortCoordinates = GetViewPortCoordinates();
                },
                GetDispatcherScheduler())
            .Subscribe(_ => { }, e => _logger.LogError($"There was an issue syncing the view port from '{component}'", e));
    }

    /// <summary>
    /// Try to push a new ViewPort, catching all exceptions to avoid killing the subscription
    /// </summary>
    private async Task TrySetViewPort(CancellationToken ct, MapViewPort viewPort)
    {
        try
        {
            _isAnimating = true;
            await SetViewPort(ct, viewPort);
            _isAnimating = false;
        }
        catch (TaskCanceledException)
        {
            // This is a normal case, for instance if the user drags the map during the animation
            _isAnimating = false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error due to the selection of many view ports. Disposed the previous one.", ex);
        }
    }
    #endregion

    #region Pushpins
    private IDisposable SyncPushpinsFrom(ViewModelBase vm)
    {
        return Observable
            .CombineLatest(
                vm.GetAndObservePushpins(filterOutChangesFromSource: this),
                vm.GetAndObserveGroups(filterOutChangesFromSource: this),
                vm.GetAndObserveSelectedPushpins(filterOutChangesFromSource: null),
                (pushpins, groups, selected) => new
                {
                    items = pushpins
                        .Concat(groups)
                        .Where(p => p.Coordinates.IsValid())
                        .ToArray(),
                    selected = selected
                }
            )
            .ObserveOn(GetDispatcherScheduler())
            .Do(x =>
            {
                UpdateMapPushpins(x.items, x.selected);
            })
            .ObserveOn(GetBackgroundScheduler())
            .CombineLatest(
                // In order to prevent the pushpins being updated when the source changes, but still making sure the pins are updated before the selection
                vm
                    .GetAndObserveSelectedPushpins(filterOutChangesFromSource: this)
                    .ObserveOn(GetDispatcherScheduler())
                    .Do(newlySelected => UpdateMapSelectedPushpins(newlySelected)),
                (values, newlySelected) => newlySelected
            )
            .Subscribe(_ => { }, e => _logger.LogError("Items sync failed", e));
    }
    #endregion

    #region SelectedPushpins
    private readonly Subject<IGeoLocated[]> _selectedPushpins = new Subject<IGeoLocated[]>();

    private IDisposable SyncSelectedPushpinsTo(ViewModelBase vm)
    {

        var component = (IDynamicMapComponent)vm;
#if WINDOWS
		return (_pushpinIcons != null ? _pushpinIcons.ObserveSelected() : _selectedPushpins)
#else
        return _selectedPushpins
#endif
            .DistinctUntilChanged()
            .ObserveOn(GetBackgroundScheduler())
            .Do(items =>
                component.SelectedPushpins = FlattenGroupings(items))
            .Subscribe(_ => { }, e => _logger.LogInformation("SyncSelectedPushpinsTo", e));
    }

    private IGeoLocated[] FlattenGroupings(IGeoLocated[] items)
    {
        var result = new List<IGeoLocated>();

        foreach (var item in items)
        {
            var grouping = item as IGeoLocatedGrouping;

            if (grouping == null)
            {
                result.Add(item);
            }
            else
            {
                result.AddRange(grouping);
            }
        }

        return result.ToArray();
    }

    #endregion

    #region IsUserDragging
    private readonly Subject<bool> _isUserDragging = new Subject<bool>();

    private IDisposable SyncIsUserDragging(ViewModelBase vm)
    {
        var component = (IDynamicMapComponent)vm;
        return _isUserDragging
            .DistinctUntilChanged()
            .ObserveOn(GetBackgroundScheduler())
            .Subscribe(isDragging => component.IsUserDragging = isDragging, e => _logger.LogError("SyncIsUserDragging", e));
    }
    #endregion

    protected virtual void SetAnimationDuration(double? animationDurationSeconds)
    {
        // Not supported by default (only supported on iOS MK)
    }
}
#endif
