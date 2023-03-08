#if __ANDROID__
using System;
using Cartography.StaticMap.Provider;
using Uno.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Image = Microsoft.UI.Xaml.Controls.Image;
using Size = Windows.Foundation.Size;

namespace Cartography.StaticMap
{
	/// <summary>
	/// Implementation of <see href="StaticMapControl"/>  in Android/>
	/// </summary>
	public partial class StaticMapControl : Control
	{
		private const string InnerMapImageElementName = "PART_InnerMap";
		private const string InnerMapContentElementName = "PART_InnerMapContentControl";

		private Size _measuredSize;
		private Image _innerMapImage;
		private ContentPresenter _innerMapContent;

		/// <inheritdoc/>
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_innerMapImage = this.GetTemplateChild(InnerMapImageElementName) as Image;
			if (_innerMapImage != null)
			{
				_innerMapImage.Stretch = Stretch.UniformToFill;
			}

			_innerMapContent = this.GetTemplateChild(InnerMapContentElementName) as ContentPresenter;

			GoToState(DefaultState);

			_viewReady = true;

			RequestUpdate();
		}

		/// <inheritdoc/>
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

			// Skip any measure done until the view is ready.
			if (!_viewReady)
			{
				return;
			}

			_measuredSize = new Size(MeasureSpec.GetSize(widthMeasureSpec), MeasureSpec.GetSize(heightMeasureSpec));
		}

		private void SetMap(object map, StaticMapParameters parameters = null)
		{
			if (map == null)
			{
				return;
			}

			if (map is Uri uri && _innerMapImage != null)
			{
				_innerMapImage.Source = uri;
			}
			else if (map is ImageSource imageSource && _innerMapImage != null)
			{
				_innerMapImage.Source = imageSource;
			}
			else if (_innerMapContent != null)
			{
				_innerMapContent.Content = map;
			}
		}

		private Size GetControlSize()
		{
			var size = ViewHelper.PhysicalToLogicalPixels(_measuredSize);

			// we round the size because this will be sent to an API that expects dimensions to be integers
			var roundedSize = new Size(
				(int)Math.Round(size.Width),
				(int)Math.Round(size.Height)
			);

			return roundedSize;
		}
	}
}
#endif
