﻿using System;
using System.Collections.Generic;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Views;

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
		// TODO: Add your ViewModel and Page associations here.

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
