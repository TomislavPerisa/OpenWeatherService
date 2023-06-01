using Newtonsoft.Json;
using OpenWeatherDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeatherDataHandler
{
    public static class WeatherDataHandler
    {
        public static List<WeatherInfo.Root> GetWeather(List<LocationInfo.Location> locations)
        {
            if (locations != null)
            {
                List<WeatherInfo.Root> weatherInfoList = new List<WeatherInfo.Root>();
                string apiKey = "e96c2407084e6dd7970ecce8a6c5a838";
                foreach(var location in locations)
                {
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={location.Name}&appid={apiKey}";
                    using (WebClient web = new WebClient())
                    {
                        var json = web.DownloadString(url);
                        WeatherInfo.Root weatherInfo = JsonConvert.DeserializeObject<WeatherInfo.Root>(json);
                        weatherInfo.id = (int)location.Id;
                        weatherInfo.SnapshotTime= DateTime.Now;
                        weatherInfo.sys.sunriseTime = ConvertDateTime(weatherInfo.sys.sunset);
                        weatherInfo.sys.sunsetTime = ConvertDateTime(weatherInfo.sys.sunset);
                        weatherInfoList.Add(weatherInfo);
                    }
                }
                return weatherInfoList;
            }
            return null;
        }

        public static DateTime ConvertDateTime(long seconds)
        {
            DateTime day = new DateTime(1970,1,1,0,0,0,0);
            day = day.AddSeconds(seconds).ToLocalTime();
            return day;
        }
    }
}
