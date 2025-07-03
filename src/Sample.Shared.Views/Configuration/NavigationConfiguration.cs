using System;
using System.Collections.Generic;
using Sample.Presentation;
using Sample.Views.Content;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Views;

/// <summary>
/// This class is used for navigation configuration.
/// - Configures the navigator.
/// </summary>
public static class NavigationConfiguration
{
	public static IServiceCollection AddNavigation(this IServiceCollection services)
	{
		return services.AddSingleton<ISectionsNavigator>(s =>
			new FrameSectionsNavigator(
				App.Instance.NavigationMultiFrame,
				GetPageRegistrations()
			)
		);
	}

	private static IReadOnlyDictionary<Type, Type> GetPageRegistrations() => new Dictionary<Type, Type>()
	{
		{ typeof(StaticMapPageViewModel), typeof(StaticMapPage) },
		{ typeof(DynamicMap_FeaturesPageViewModel), typeof(DynamicMap_FeaturesPage) },
		{ typeof(DynamicMap_MoveSearchPageViewModel), typeof(DynamicMap_MoveSearchPage) },
		{ typeof(GoogleMapsControl_FeaturesPageViewModel), typeof(GoogleMapControl_FeaturesPage) },
		{ typeof(MainPageViewModel), typeof(MainPage) },
		{ typeof(LocationViewModel), typeof(LocationPage) },
		{ typeof(MapServiceViewModel), typeof(MapServicePage) },
		{ typeof(DynamicMapMenuViewModel), typeof(DynamicMapMenuPage) },

	};

	/// <summary>
	/// Disable navigation animations.
	/// </summary>
	/// <remarks>
	/// Do not remove even if it's not used by default.
	/// </remarks>
	/// <param name="frameSectionsNavigator"><see cref="FrameSectionsNavigator"/>.</param>
	private static void DisableAnimations(FrameSectionsNavigator frameSectionsNavigator)
	{
		frameSectionsNavigator.DefaultSetActiveSectionTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		frameSectionsNavigator.DefaultOpenModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
		frameSectionsNavigator.DefaultCloseModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
	}
}
