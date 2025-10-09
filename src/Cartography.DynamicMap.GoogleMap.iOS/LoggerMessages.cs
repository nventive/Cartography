using Microsoft.Extensions.Logging;

namespace Cartography.DynamicMap.GoogleMap.iOS;

internal static partial class LoggerMessages
{
	[LoggerMessage(EventId = 3000, Level = LogLevel.Debug, Message = "Setting the ViewPort to {ViewPort}.")]
	public static partial void SettingViewPort(ILogger logger, MapViewPort viewPort);

	[LoggerMessage(EventId = 3001, Level = LogLevel.Trace, Message = "Fitting to points of interest.")]
	public static partial void FittingToPointsOfInterest(ILogger logger);

	[LoggerMessage(EventId = 3002, Level = LogLevel.Trace, Message = "Including the center point {Center} in the points of interest bounds calculation.")]
	public static partial void IncludingCenterInBounds(ILogger logger, Geopoint center);

	[LoggerMessage(EventId = 3003, Level = LogLevel.Trace, Message = "Setting zoom level to {ZoomLevel}.")]
	public static partial void SettingZoomLevel(ILogger logger, ZoomLevel? zoomLevel);

	[LoggerMessage(EventId = 3004, Level = LogLevel.Trace, Message = "Setting center to {Center}.")]
	public static partial void SettingCenter(ILogger logger, Geopoint center);

	[LoggerMessage(EventId = 3005, Level = LogLevel.Trace, Message = "Animating to new position.")]
	public static partial void AnimatingToNewPosition(ILogger logger);

	[LoggerMessage(EventId = 3006, Level = LogLevel.Trace, Message = "Moving to new position without animation.")]
	public static partial void MovingWithoutAnimation(ILogger logger);

	[LoggerMessage(EventId = 3007, Level = LogLevel.Information, Message = "Successfuly set the ViewPort to {ViewPort}.")]
	public static partial void SuccessfullySetViewPort(ILogger logger, MapViewPort viewPort);

	[LoggerMessage(EventId = 3008, Level = LogLevel.Debug, Message = "The view port changed (Idle).")]
	public static partial void ViewPortChangedIdle(ILogger logger);

	[LoggerMessage(EventId = 3009, Level = LogLevel.Error, Message = "Pushpins icons cannot be changed.")]
	public static partial void PushpinIconsCannotBeChanged(ILogger logger);

	[LoggerMessage(EventId = 3010, Level = LogLevel.Debug, Message = "Unselecting all the {Count} pushpins.")]
	public static partial void UnselectingAllPushpins(ILogger logger, int count);

	[LoggerMessage(EventId = 3011, Level = LogLevel.Information, Message = "Unselected all the {Count} pushpins.")]
	public static partial void UnselectedAllPushpins(ILogger logger, int count);

	[LoggerMessage(EventId = 3012, Level = LogLevel.Debug, Message = "Updating the pushpin.")]
	public static partial void UpdatingPushpin(ILogger logger);

	[LoggerMessage(EventId = 3013, Level = LogLevel.Information, Message = "Updated the pushpins.")]
	public static partial void UpdatedPushpins(ILogger logger);
}
