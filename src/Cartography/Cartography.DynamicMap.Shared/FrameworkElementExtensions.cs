using System;
using System.Reflection;
using Uno.Extensions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive;
#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;
#if WINDOWS
using Microsoft.UI.Xaml.Data;
#elif __ANDROID__ || __IOS__ || __WASM__
using Microsoft.UI.Xaml.Data;
using Uno.UI.DataBinding;
#else
using System.Windows;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
#if WINDOWS_UWP
using Windows.UI.Xaml.Data;
#elif __ANDROID__ || __IOS__ || __WASM__
using Windows.UI.Xaml.Data;
using Uno.UI.DataBinding;
#else
using System.Windows;
#endif
#endif

namespace Cartography.DynamicMap
{
	internal static partial class FrameworkElementExtensions
	{
		[Flags]
		public enum UiEventSubscriptionsOptions
		{
			/// <summary>
			/// Default is ImmediateSubscribe
			/// </summary>
			Default = ImmediateSubscribe,

			/// <summary>
			/// Subscribe and Unsubscribe will be enforced on Dispacther scheduler
			/// </summary>
			/// <remarks>Be sure to not miss an event between subscribe to observable and real event handler add</remarks>
			DispatcherOnly = 0,

			/// <summary>
			/// Add event handler immediatly on Subscribe.
			/// </summary>
			/// <remarks>This mean you must call subscribe on dispatcher</remarks>
			ImmediateSubscribe = 1,

			/// <summary>
			/// Remove event handler immediatly on Dispose / Complete.
			/// </summary>
			/// <remarks>This mean you must dispose subscription on dispatcher</remarks>
			ImmediateUnsubscribe = 2
		}

		/// <summary>
		/// Helper to create an observable sequence "FromEventPattern" on a DependencyObject using right scheduler.
		/// </summary>
		internal static IObservable<EventPattern<TArgs>> FromEventPattern<THandler, TArgs>(
			Action<THandler> addHandler,
			Action<THandler> removeHandler,
			DependencyObject element,
			UiEventSubscriptionsOptions options)
#if !WINDOWS_UWP && !WINDOWS && !__ANDROID__ && !__IOS__ && !__WASM__
			where TArgs : EventArgs 
#endif
		{
			var immediateSubscribe = options.HasFlag(UiEventSubscriptionsOptions.ImmediateSubscribe);
			var immediateUnsubscribe = options.HasFlag(UiEventSubscriptionsOptions.ImmediateUnsubscribe);
			return Observable.FromEventPattern<THandler, TArgs>(addHandler, removeHandler, Scheduler.Immediate);

		}

#if __ANDROID__ || __IOS__
		public static T Style<T>(this T element, Style style) where T : FrameworkElement
		{
			element.Style = style;
			return element;
		}

		public static T Binding<T>(this T element, string property, string propertyPath, string converter) where T : IDependencyObjectStoreProvider
		{
			var dependencyProperty = GetDependencyProperty(element, property);
			var path = new PropertyPath(propertyPath.Replace("].[", "]["));
			var binding = new Binding { Path = path, Converter = ResourceHelper.FindConverter(converter) };

			(element as IDependencyObjectStoreProvider).Store.SetBinding(dependencyProperty, binding);

			return element;
		}

		public static T Binding<T>(this T element, string property, string propertyPath, object source, BindingMode mode) where T : DependencyObject
		{
			return element.Binding(property,
				new Binding()
				{
					Path = propertyPath,
					Source = source,
					Mode = mode
				}
			);
		}

		public static T Binding<T>(this T element, string property, BindingBase binding) where T : DependencyObject
		{
#if WINDOWS_UWP || WINDOWS
			var dependencyProperty = GetDependencyProperty(element, property);

			element.SetBinding(dependencyProperty, binding);
#else
			(element as IDependencyObjectStoreProvider).Store.SetBinding(property, binding);
#endif

			return element;
		}

		private static DependencyProperty GetDependencyProperty(object element, string propertyName)
		{
			var dependencyProperty = GetDependencyPropertyFromProperties(element, propertyName);

			if (dependencyProperty == null)
			{
				dependencyProperty = GetDependencyPropertyFromFields(element, propertyName);
			}

			if (dependencyProperty == null)
			{
				throw new InvalidOperationException("Unable to find the dependency property [{0}]".InvariantCultureFormat(propertyName));
			}

			return dependencyProperty;
		}
		private static DependencyProperty GetDependencyPropertyFromFields(object element, string property)
		{
			FieldInfo fieldInfo = null;
			var currentType = element.GetType();
			do
			{
				fieldInfo = currentType.GetTypeInfo().GetDeclaredField(property + "Property");

				if (fieldInfo == null)
				{
					currentType = currentType.GetTypeInfo().BaseType;
				}

			}
			while (currentType != null && fieldInfo == null);

			if (fieldInfo != null)
			{
				var dependencyProperty = (DependencyProperty)fieldInfo.GetValue(null);
				return dependencyProperty;
			}

			return null;
		}

		private static DependencyProperty GetDependencyPropertyFromProperties(object element, string property)
		{
			PropertyInfo propertyInfo = null;
			var currentType = element.GetType();

			do
			{
				propertyInfo = currentType.GetTypeInfo().GetDeclaredProperty(property + "Property");

				if (propertyInfo == null)
				{
					currentType = currentType.GetTypeInfo().BaseType;
				}

			} while (currentType != null && propertyInfo == null);

			if (propertyInfo != null)
			{
				var dependencyProperty = (DependencyProperty)propertyInfo.GetMethod.Invoke(null, new object[0]);
				return dependencyProperty;
			}


			return null;
		}

		public static T Binding<T>(this T element, string property, string propertyPath) where T : DependencyObject
		{
			var path = new PropertyPath(propertyPath.Replace("].[", "]["));
			var binding = new Binding { Path = path };

			return element.Binding(property, binding);
		}

		public static Run Binding(
			this Run element,
			string property,
			string propertyPath
		)
		{
			propertyPath = propertyPath.Replace("].[", "][");

			if (property == "Text")
			{
				var templateString = "<Run xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Text=\"{{Binding {0}}}\" />";

				return (Run)XamlReader.Load(templateString.InvariantCultureFormat(propertyPath));
			}
			else
			{
				var path = new PropertyPath(propertyPath);
				var binding = new Binding { Path = path };

				var dependencyProperty = GetDependencyProperty(element, property);

				BindingOperations.SetBinding(element, dependencyProperty, binding);

				return element;
			}
		}
#endif
	}
}
