namespace Samples.ProjectTest.DataContracts
{
    /// <summary>
    /// A Weather Forecast for a specific date
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Gets the Date of the Forecast.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets the Forecast Temperature in Celsius.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Get a description of how the weather will feel.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Gets the Forecast Temperature in Fahrenheit
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}