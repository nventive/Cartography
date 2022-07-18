namespace Cartography.Core
{
	public class ViewPortPadding
	{
		private const double DefaultPushpinsBoundsPadding = 1.1;
		private double? _horizontalPadding;
		private double? _verticalPadding;

		public ViewPortPadding()
		{
			_horizontalPadding = DefaultPushpinsBoundsPadding;
			_verticalPadding = DefaultPushpinsBoundsPadding;
		}

		public ViewPortPadding(double horizontalPadding, double verticalPadding)
		{
			_horizontalPadding = horizontalPadding;
			_verticalPadding = verticalPadding;
		}

		/// <summary>
		/// Gets or sets an optional horizontal padding on the ViewPort to make sure pushpins are not shown partially.
		/// This value represent a coefficient. The padding applied to the ViewPort will be the coefficient times the calculated width.
		/// </summary>
		public double? HorizontalPadding
		{
			get => _horizontalPadding ?? DefaultPushpinsBoundsPadding;
			set => _horizontalPadding = value;
		}

		/// <summary>
		/// Gets or sets an optional vertical padding on the ViewPort to make sure pushpins are not shown partially.
		/// This value represent a coefficient. The padding applied to the ViewPort will be the coefficient times the calculated height.
		/// </summary>
		public double? VerticalPadding
		{
			get => _verticalPadding ?? DefaultPushpinsBoundsPadding;
			set => _verticalPadding = value;
		}
	}
}
