namespace Cartography.shared.Helpers
{
    internal static class ValidationExtensionsHelpers
    {
        public static ValidationExtensionPoint<T> Validation<T>(this T value) where T : class
        {
            return new ValidationExtensionPoint<T>(value);
        }
    }
}