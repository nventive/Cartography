﻿using System;
using System.Reflection;

#if WINDOWS_UWP
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using GenericCulture = System.String;
#elif __ANDROID__ || __IOS__ || __WASM__
using Windows.UI.Xaml.Data;
using GenericCulture = System.String;
#else
using System.Windows.Data;
using GenericCulture = System.Globalization.CultureInfo;
#endif

namespace Samples.Views
{
    public abstract class ConverterBase : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, GenericCulture culture)
        {
            return Convert(value, targetType, parameter);
        }

        protected abstract object Convert(object value, Type targetType, object parameter);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, GenericCulture culture)
        {
            return ConvertBack(value, targetType, parameter);
        }

        protected virtual object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotSupportedException();
        }
    }
}
