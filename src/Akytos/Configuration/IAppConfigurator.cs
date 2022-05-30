namespace Akytos.Configuration;

public interface IAppConfigurator
{
    IAppConfigurator ConfigureGame(Action<IConfigureGame> configure);
    IAppConfigurator ConfigureLayers(Action<IConfigureLayers> configure);
}