namespace Cartography.shared.Helpers
{
    public static class ValidationExtensionsBase
    {
        public static ValidationExtensionPoint<T> Validation<T>(this T value) where T : class
        {
            return new ValidationExtensionPoint<T>(value);
        }
    }
}