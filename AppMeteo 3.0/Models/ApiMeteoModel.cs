namespace AppMeteo_3._0.Models
{
    public class ApiMeteoModel
    {

        public class LocationResponse
        {
            public string? Key { get; set; }
            public string? CityName { get; set; }
        }

        public class JsonResponse
        {
            public Temperature? Temperature { get; set; }
            public string? WeatherText { get; set; }
        }

        public class Temperature
        {
            public Metric? Metric { get; set; }
        }

        public class Metric
        {
            public double Value { get; set; }
            public string? Unit { get; set; }
        }

    }
}
