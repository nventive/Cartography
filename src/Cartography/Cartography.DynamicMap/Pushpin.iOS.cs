#if __IOS__

using System.ComponentModel;

namespace Cartography.DynamicMap
{
	[Windows.UI.Xaml.Data.Bindable]
	public class Pushpin : MapControlItem, ISelectable
	{
		public const string AnnotationId = "Nventive.Pushpin";

		private bool _isSelected;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					if (!IsSelectionChangeAlreadyHandled)
					{
						if (value)
						{
							Map.SelectAnnotation(this, true);
						}
						else
						{
							Map.DeselectAnnotation(this, true);
						}
					}
					RaisePropertyChanged();
				}
			}
		}

		public int ZIndex { get; set; }

		/// <summary>
		/// This means the selection/deselection of this pin
		/// is already handled and shouldn't trigger
		/// additional selection/deselection events (avoid infinite loops).
		/// </summary>
		[DefaultValue (false)]
		public bool IsSelectionChangeAlreadyHandled { 
			get;
			set; 
		}
	}
}
#endif
