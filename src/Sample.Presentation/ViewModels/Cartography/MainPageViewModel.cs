﻿using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sample.Presentation
{
	[Bindable(true)]
	public class MainPageViewModel : ViewModel
    {
        private readonly ISectionsNavigator _sectionsNavigator;

        public MainPageViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();

        }
        public IDynamicCommand NavigateToMapService => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.NavigateAndClear(ct, () => new MapServiceViewModel());
        });

        public IDynamicCommand NavigateToLocation => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.NavigateAndClear(ct, () => new LocationViewModel());
        });

        public IDynamicCommand NavigateToDynamicMap => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.NavigateAndClear(ct, () => new DynamicMapMenuViewModel());
        });

        public IDynamicCommand NavigateToStaticMap => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.NavigateAndClear(ct, () => new StaticMapPageViewModel());
        });
    }
}
