namespace WebAPI
{
    
    /// Template model used by the default weather forecast endpoint.
   
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        
        /// Performs the TemperatureF operation for this class.
       
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
