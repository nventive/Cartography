using System;
using Uno.Validation;

namespace Cartography.DynamicMap.Helpers
{
    public static class ValidationExtensions
    {
        public static ValidationExtensionPoint<T> Validation<T>(this T value) where T : class
        {
            return new ValidationExtensionPoint<T>(value);
        }

        public static T NotNull<T>(this ValidationExtensionPoint<T> extensionPoint, string name) where T : class
        {
            if (extensionPoint.ExtendedValue == null)
            {
                throw new ArgumentNullException(name);
            }

            return extensionPoint.ExtendedValue;
        }
        public static T Found<T>(this ValidationExtensionPoint<T> extensionPoint) where T : class
        {
            if (extensionPoint.ExtendedValue == null)
            {
                throw new ArgumentException();
                //throw new NotFoundException();
            }

            return extensionPoint.ExtendedValue;
        }
        public static void NotSupported<T>(this ValidationExtensionPoint<T> extensionPoint) where T : class
        {
            throw new NotSupportedException();
        }

        public static T IsTrue<T>(this ValidationExtensionPoint<T> extensionPoint, Func<T, bool> validation, string paramName, string message = null) where T : class
        {
            if (!validation(extensionPoint.ExtendedValue))
            {
                throw new ArgumentException(message);
            }

            return extensionPoint.ExtendedValue;
        }

        public static T IsFalse<T>(this ValidationExtensionPoint<T> extensionPoint, Func<T, bool> validation, string paramName, string message = null) where T : class
        {
            if (validation(extensionPoint.ExtendedValue))
            {
                throw new ArgumentException(message);
            }

            return extensionPoint.ExtendedValue;
        }
    }
} 