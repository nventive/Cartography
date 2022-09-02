﻿#if __IOS__
using System;
using Google.Maps;
using Microsoft.Extensions.Logging;
using Cartography.StaticMap.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Cartography.StaticMap
{
	/// <summary>
	/// Implementation of <see href="StaticMapControl"/>  in IOS/>
	/// </summary>
	public partial class StaticMapControl : Control
	{
		private const string InnerMapElementName = "PART_InnerMapContentControl";
		private Image _image;
		private ContentPresenter _innerMap;
		private ILogger _logger;

		/// <inheritdoc/>
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_image = new Image();
			_image.Stretch = Stretch.UniformToFill;

			_innerMap = (this.GetTemplateChild(InnerMapElementName) as ContentPresenter).Validation().NotNull(InnerMapElementName);

			GoToState(DefaultState);

			_viewReady = true;

			RequestUpdate();
		}

		private void SetMap(object map, StaticMapParameters parameters = null)
		{
			_logger.LogDebug($"Setting map (map: '{map?.GetType()}', height: '{parameters?.Height}', width: '{parameters?.Width}', scale: '{parameters?.Scale}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}').");

			if (map == null)
			{
				_logger.LogInformation("Map not set because it is null.");

				return;
			}

			if (map is ImageSource imageSource)
			{
				_image.Source = imageSource;
				_innerMap.Content = _image;
			}
			else if (map is Uri uri)
			{
				_image.Source = uri;
				_innerMap.Content = _image;
			}
			else if (map is MapView && parameters != null)
			{
				_innerMap.Height = parameters.Height;
				_innerMap.Width = parameters.Width;

				_innerMap.Content = map;
			}

			_logger.LogInformation($"Set map (map: '{map.GetType()}', height: '{parameters?.Height}', width: '{parameters?.Width}', scale: '{parameters?.Scale}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}' ).");
		}
	}
}
#endif
