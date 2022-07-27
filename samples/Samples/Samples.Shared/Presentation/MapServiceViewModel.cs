using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Presentation
{
    public partial class MapServiceViewModel : ViewModel
    {

        private ISectionsNavigator _sectionsNavigator;
        
        public MapServiceViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
        }

        public IDynamicCommand MapServiceToMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });
    }
}
