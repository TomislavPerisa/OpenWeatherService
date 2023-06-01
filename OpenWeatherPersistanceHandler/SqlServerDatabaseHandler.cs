using OpenWeatherDataHandler.Models;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace OpenWeatherPersistanceHandler
{
    public static class SqlServerDatabaseHandler
    {
        private static string _dbConnection = "Data Source=.;Initial Catalog=WeatherDB;Integrated Security=True;Pooling=False;TrustServerCertificate=True";
        private static void ExecuteSqlQuery(List<SqlCommand> sqlCommands, SqlConnection connection)
        {
            if (sqlCommands != null && connection != null)
            {
                connection.Open();
                foreach (var sqlCommand in sqlCommands)
                {
                    sqlCommand.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static List<LocationInfo.Location> GetCities()
        {
            List<LocationInfo.Location> cities = new List<LocationInfo.Location>();
            SqlConnection con = new SqlConnection(_dbConnection);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from cities", con);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    cities.Add(new LocationInfo.Location
                    {
                        InternalId = int.Parse(reader["Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        Country = reader["Country"].ToString(),
                        Id = float.Parse(reader["IdExternal"].ToString())
                    });
                }
            }
            con.Close();
            return cities;
        }

        public static void AddCities(List<LocationInfo.Location> locations)
        {
            if (locations != null)
            {
                List<LocationInfo.Location> cities = GetCities();
                List<SqlCommand> sqlCommands = new List<SqlCommand>();
                SqlConnection con = new SqlConnection(_dbConnection);
                foreach (var location in locations)
                {
                    if (!cities.Exists(x => x.Name == location.Name))
                    {
                        var query = $"Insert into cities values(@Name, @Country, @IdExternal)";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Name", location.Name);
                        cmd.Parameters.AddWithValue("@Country", location.Country);
                        cmd.Parameters.AddWithValue("@IdExternal", (int)location.Id);
                        sqlCommands.Add(cmd);
                    }
                }
                ExecuteSqlQuery(sqlCommands, con);
            }
        }

        public static void SaveWeatherInfo(List<WeatherInfo.Root> weatherInfos)
        {
            if (weatherInfos != null)
            {
                List<LocationInfo.Location> cities = GetCities();
                SqlConnection con = new SqlConnection(_dbConnection);
                int weatherDatasId = 0;
                var query = "";

                con.Open();
                foreach (var weatherInfo in weatherInfos)
                {
                    query = $"Insert into weatherDatas values (@SnapshotTime,@All,@Lon,@Lat,@Temp,@Feels_like,@Temp_min,@Temp_max,@Pressure,@Humidity,@Rain1h,@Rain3h,@Snow1h,@Snow3h,@Base,@Visibility,@Dt,@Type,@SunriseTime,@SunsetTime,@Speed,@Deg,@IdCity); ";
                    query += $"Select CAST(SCOPE_IDENTITY() AS INT); ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SnapshotTime", weatherInfo.SnapshotTime);
                    cmd.Parameters.AddWithValue("@All", weatherInfo.clouds.all);
                    cmd.Parameters.AddWithValue("@Lon", weatherInfo.coord.lon);
                    cmd.Parameters.AddWithValue("@Lat", weatherInfo.coord.lat);
                    cmd.Parameters.AddWithValue("@Temp", weatherInfo.main.temp);
                    cmd.Parameters.AddWithValue("@Feels_like", weatherInfo.main.feels_like);
                    cmd.Parameters.AddWithValue("@Temp_min", weatherInfo.main.temp_min);
                    cmd.Parameters.AddWithValue("@Temp_max", weatherInfo.main.temp_max);
                    cmd.Parameters.AddWithValue("@Pressure", weatherInfo.main.pressure);
                    cmd.Parameters.AddWithValue("@Humidity", weatherInfo.main.humidity);
                    cmd.Parameters.AddWithValue("@Rain1h", weatherInfo.rain?._1h ?? SqlDouble.Null);
                    cmd.Parameters.AddWithValue("@Rain3h", weatherInfo.rain?._3h ?? SqlDouble.Null);
                    cmd.Parameters.AddWithValue("@Snow1h", weatherInfo.snow?._1h ?? SqlDouble.Null);
                    cmd.Parameters.AddWithValue("@Snow3h", weatherInfo.snow?._3h ?? SqlDouble.Null);
                    cmd.Parameters.AddWithValue("@Base", weatherInfo.@base);
                    cmd.Parameters.AddWithValue("@Visibility", weatherInfo.visibility);
                    cmd.Parameters.AddWithValue("@Dt", weatherInfo.dt);
                    cmd.Parameters.AddWithValue("@Type", weatherInfo.sys.type);
                    cmd.Parameters.AddWithValue("@SunriseTime", weatherInfo.sys.sunriseTime);
                    cmd.Parameters.AddWithValue("@SunsetTime", weatherInfo.sys.sunsetTime);
                    cmd.Parameters.AddWithValue("@Speed", weatherInfo.wind.speed);
                    cmd.Parameters.AddWithValue("@Deg", weatherInfo.wind.deg);
                    cmd.Parameters.AddWithValue("@IdCity", cities.FirstOrDefault(x => x.Id == weatherInfo.id).InternalId);

                    weatherDatasId = (int)cmd.ExecuteScalar();

                    query = $"Insert into weatherDetails values(@Main,@Description,@IconUrl,@IdWeatherData)";
                    foreach (var weatherDetail in weatherInfo.weather)
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@Main", weatherDetail.main);
                        cmd.Parameters.AddWithValue("@Description", weatherDetail.description);
                        cmd.Parameters.AddWithValue("@IconUrl", weatherDetail.iconUrl);
                        cmd.Parameters.AddWithValue("@IdWeatherData", weatherDatasId);
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
        }
    }
}
