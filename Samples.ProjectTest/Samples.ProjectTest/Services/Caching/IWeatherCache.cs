using Samples.ProjectTest.DataContracts;
using System.Collections.Immutable;

namespace Samples.ProjectTest.Services.Caching
{
    public interface IWeatherCache
    {
        ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
    }
}