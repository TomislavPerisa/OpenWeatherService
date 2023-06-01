using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using OpenWeatherDataHandler;
using OpenWeatherDataHandler.Models;
using OpenWeatherPersistanceHandler;

namespace OpenWeatherService
{
    public class OpenWeatherRequests
    {
        private readonly System.Timers.Timer _timer;

        public OpenWeatherRequests()
        {
            _timer = new System.Timers.Timer(300000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Stop();
            CitiesHandler();
            GetWeatherData();
            Start();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void CitiesHandler()
        {
            var exists = (SqlServerDatabaseHandler.GetCities().Count > 0) ? true : false;
            if (!exists)
            {
                List<LocationInfo.Location> locationInfo = LocationDataHandler.GetAllLocations();
                List<LocationInfo.Location> locationsCro = new List<LocationInfo.Location>();
                foreach (LocationInfo.Location item in locationInfo)
                {
                    if (item.Country == "HR") locationsCro.Add(item);
                }
                SqlServerDatabaseHandler.AddCities(locationsCro);
            }
        }

        private void GetWeatherData()
        {
            List<LocationInfo.Location> locationInfo = SqlServerDatabaseHandler.GetCities();
            var weatherInfos = WeatherDataHandler.GetWeather(locationInfo);
            SqlServerDatabaseHandler.SaveWeatherInfo(weatherInfos);
        }
    }
}
