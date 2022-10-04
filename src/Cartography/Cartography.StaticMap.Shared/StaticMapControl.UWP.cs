#if WINDOWS_UWP || WINDOWS
using System;
using Cartography.StaticMap.Provider;
using Windows.Foundation;
#if WINUI
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Cartography.StaticMap
{
	/// <summary>
	/// Implementation of <see href="StaticMapControl"/>  in Windows/>
	/// </summary>
	public partial class StaticMapControl : Control
	{
		private const string InnerMapElementName = "PART_InnerMapContentControl";
		private Image _image;
		private ContentPresenter _innerMap;
		private Size _measuredSize;

		/// <inheritdoc/>
		protected override Size MeasureOverride(Size availableSize)
		{
			if (!UseFixedMapSize() && _viewReady)
			{
				_measuredSize = availableSize;
			}

			return base.MeasureOverride(availableSize);
		}

		/// <inheritdoc/>
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_image = new Image();
			_image.Stretch = Stretch.UniformToFill;

			_innerMap = this.GetTemplateChild(InnerMapElementName) as ContentPresenter;

			GoToState(DefaultState);

			_viewReady = true;

			RequestUpdate();
		}

		partial void InnerConstruct()
		{
			DefaultStyleKey = typeof(StaticMapControl);
		}

		private void SetMap(object map, StaticMapParameters parameters = null)
		{
			_image.Source = map as ImageSource;
			if (_image.Source == null && map != null)
			{
				_image.Source = new BitmapImage(map as Uri);
			}

			_innerMap.Content = _image.Source != null ? _image : null;
		}

		private Size GetControlSize()
		{
			return _measuredSize;
		}
	}
}
#endif
