using OpenWeatherService;
using Topshelf;

var exitCode = HostFactory.Run(x =>
{
    x.Service<OpenWeatherRequests>(s =>
    {
        s.ConstructUsing(ow => new OpenWeatherRequests());
        s.WhenStarted(ow => ow.Start());
        s.WhenStopped(ow => ow.Stop());
    });

    x.RunAsLocalSystem();

    x.SetServiceName("OpenWeatherService");
    x.SetDisplayName("Open weather service");
    x.SetDescription("Service for retrieval of weather data from open weather api. Service is storing data in a database.");
});

int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
Environment.ExitCode = exitCodeValue;