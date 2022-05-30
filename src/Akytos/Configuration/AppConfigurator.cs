namespace Akytos.Configuration;

public class AppConfigurator : IAppConfigurator
{
    private readonly Application m_application;

    public AppConfigurator(Application application)
    {
        m_application = application;
    }

    public IAppConfigurator ConfigureLayers(Action<IConfigureLayers> configure)
    {
        configure(m_application);
        
        return this;
    }

    public IAppConfigurator ConfigureGame(Action<IConfigureGame> configure)
    {
        configure(m_application);
        
        return this;
    }
}