using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Presentation
{
    public partial class DynamicMapMenuViewModel : ViewModel
    {

        private readonly ISectionsNavigator _sectionsNavigator;

        public DynamicMapMenuViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
        }

        public IDynamicCommand DynamicMenuToMainMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });

        public IDynamicCommand GotoDynamicMap_FeaturesPage => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new DynamicMap_FeaturesPageViewModel());
        });

        public IDynamicCommand GotoDynamicMap_MoveSearchPage => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new DynamicMap_MoveSearchPageViewModel());
        });

        public IDynamicCommand GotoDynamicMap_ZoomPoiPage => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new DynamicMap_ZoomPoiPageViewModel());
        });

        public IDynamicCommand GotoPretty_PushpinSelectionPage => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });

    }
}
