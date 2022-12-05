namespace GreatJorb.Tests;

public class TestServiceProvider : IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    public TestServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static TestServiceProvider CreateServiceProvider(
        bool includeConfiguration=false,
        bool includeMediator=false,
        bool includePuppeteer=false)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                c.AddUserSecrets<TestServiceProvider>();
            })
           .ConfigureServices(sc =>
           {
                #pragma warning disable CA1416
                if (includeConfiguration)
                    sc.AddSingleton<ISettingsService, SettingsService>();
                #pragma warning restore CA1416

               if (includeMediator)
                   sc.AddMediatR(typeof(LoginQuery));

               if (includePuppeteer)
               {
                   sc.AddSingleton<BrowserProvider>();

                   sc.AddSingleton<IWebSiteNavigator, LinkedInNavigator>();
                   sc.AddSingleton<IJobPostingExtractor, LinkedInJobPostingExtractor>();

               }
           });

       return new TestServiceProvider(builder.Build().Services);
    }

    public void Dispose()
    {
        var browserProvider = _serviceProvider.GetService<BrowserProvider>();
        if(browserProvider != null)
        {
            browserProvider.UnloadBrowser();
        }
    }

    public IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();

    public ISettingsService SettingsService => _serviceProvider.GetRequiredService<ISettingsService>();

    public IConfiguration Configuration => _serviceProvider.GetRequiredService<IConfiguration>();

}
