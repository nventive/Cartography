#if NETFX_CORE
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// A pushpin of a Map
	/// </summary>
	[TemplateVisualState(GroupName = "SelectionStates", Name = _unselectedVisualStateName)]
	[TemplateVisualState(GroupName = "SelectionStates", Name = _selectedVisualStateName)]
	public class Pushpin : MapControlItem, ISelectable
	{
		private const string _selectedVisualStateName = "Selected";
		private const string _unselectedVisualStateName = "Unselected";

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached(
			"ItemTemplate", typeof (DataTemplate), typeof (Pushpin), new PropertyMetadata(default(DataTemplate)));

		public static DataTemplate GetItemTemplate(MapControl control)
		{
			return (DataTemplate) control.GetValue(ItemTemplateProperty);
		}

		public static void SetItemTemplate(MapControl control, DataTemplate value)
		{
			control.SetValue(ItemTemplateProperty, value);
		}

		public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.RegisterAttached(
			"SelectedItemTemplate", typeof (DataTemplate), typeof (Pushpin), new PropertyMetadata(default(DataTemplate)));

		public static DataTemplate GetSelectedItemTemplate(MapControl control)
		{
			return (DataTemplate)control.GetValue(SelectedItemTemplateProperty);
		}

		public static void SetSelectedItemTemplate(MapControl control, DataTemplate value)
		{
			control.SetValue(SelectedItemTemplateProperty, value);
		}

#region IsSelected (dp)
		/// <summary>
		/// Identifies the <see cref="IsSelected"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
			"IsSelected", typeof(bool), typeof(Pushpin), new PropertyMetadata(default(bool), OnIsSelectedChanged));

		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as Pushpin)?.UpdateSelectionVisualState((bool)e.NewValue);
		}

		/// <summary>
		/// Gets or sets a bool which indicates that this pushpin is currently selected.
		/// </summary>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
#endregion

		public int ZIndex { get; set; }

		/// <summary>
		/// ctor.
		/// </summary>
		internal Pushpin(MapControlBase map)
			: base(map)
		{
			DefaultStyleKey = typeof(Pushpin);

			Loaded += (snd, e) =>
			{
				// Need to reset the state completely in case the pushpin is being re-inserted
				VisualStateManager.GoToState(this, _unselectedVisualStateName, false);
				UpdateSelectionVisualState(IsSelected, false);
			};
		}

		/// <inherit />
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			UpdateSelectionVisualState(IsSelected, false);
		}

		/// <inherit />
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			// First goto to unselected state immediatly (no animation), then change the value
			UpdateSelectionVisualState(false, false);
			IsSelected = false;

			base.OnContentChanged(oldContent, newContent);
		}

		private void UpdateSelectionVisualState(bool isSelected, bool useTransitions = true)
		{
			var stateName = isSelected ? _selectedVisualStateName : _unselectedVisualStateName;
			VisualStateManager.GoToState(this, stateName, useTransitions);
		}

		protected override void OnTapped(TappedRoutedEventArgs e)
		{
			base.OnTapped(e);

			e.Handled = Map.ToggleSelection(this);
		}
	}
}
#endif
