using System;
using System.Collections.Generic;
using System.Text;
using Samples.Presentation;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Views
{
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
            { typeof(MainPageViewModel), typeof(MainPage) },
            { typeof(DynamicMapMenuViewModel), typeof(DynamicMapMenuPage) },
            { typeof(StaticMapPageViewModel), typeof(StaticMapPage) },
            { typeof(LocationViewModel), typeof(LocationPage) },
            { typeof(DynamicMap_FeaturesPageViewModel), typeof(DynamicMap_FeaturesPage) },
            { typeof(DynamicMap_MoveSearchPageViewModel), typeof(DynamicMap_MoveSearchPage) },
            { typeof(DynamicMap_SelectedFlipViewPageViewModel), typeof(DynamicMap_SelectedFlipViewPage) },
            { typeof(Pretty_PushpinSelectionPageViewModel), typeof(DynamicMap_Pretty_PushpinSelectionPage) },
            { typeof(DynamicMap_ZoomPoiPageViewModel), typeof(DynamicMap_ZoomPoiPage) },
            { typeof(MapServiceViewModel), typeof(MapServicePage) },
        };

        private static void DisableAnimations(FrameSectionsNavigator frameSectionsNavigator)
        {
            frameSectionsNavigator.DefaultSetActiveSectionTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
            frameSectionsNavigator.DefaultOpenModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
            frameSectionsNavigator.DefaultCloseModalTransitionInfo = FrameSectionsTransitionInfo.SuppressTransition;
        }
    }
}
