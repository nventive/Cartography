﻿using Samples.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.Hosting;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Chinook.DynamicMvvm;
using System.Diagnostics;
using Samples.Views;


namespace Samples
{
    public sealed partial class DynamicMap_FeaturesPage : Page
    {
        public DynamicMap_FeaturesPage()
        {
            this.InitializeComponent();
        }
    }
}