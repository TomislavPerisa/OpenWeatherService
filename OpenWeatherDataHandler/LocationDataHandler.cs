using Newtonsoft.Json;
using OpenWeatherDataHandler.Models;

namespace OpenWeatherDataHandler
{
    public static class LocationDataHandler
    {
        public static List<LocationInfo.Location> GetAllLocations()
        {
            using (StreamReader r = new StreamReader(@"C:\Users\Tomislav\Desktop\city.list.json"))
            {
                string json = r.ReadToEnd();
                List<LocationInfo.Location> locations = JsonConvert.DeserializeObject<List<LocationInfo.Location>>(json);
                return locations;
            }
        }
    }
}
