using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System;

namespace Cartography.DynamicMap;

public static class DynamicMapConfiguration
{
    /// <summary>
    /// Gets or sets the <see cref="ILoggerFactory"/> used by all classes under the <see cref="Cartography.DynamicMap"/> namespace.
    /// The default value is a <see cref="NullLoggerFactory"/> instance.
    /// </summary>
    public static ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

    public static ILogger<T> Log<T>(this T _)
    {
        return LoggerFactory.CreateLogger<T>();
    }

    public static ILogger Log(this Type type)
    {
        return LoggerFactory.CreateLogger(type);
    }
}
