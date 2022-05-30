using Serilog;

namespace Akytos.Configuration;

public interface IAppConfigurator
{
    IAppConfigurator ConfigureGame(Action<IConfigureGame> configure);
    IAppConfigurator ConfigureLayers(Action<IConfigureLayers> configure);
    IAppConfigurator ConfigureLogging(Action<LoggerConfiguration, LoggerConfiguration> configureLogs);
}