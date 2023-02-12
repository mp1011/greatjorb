using GreatJorb.Business.Services.Settings;

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
        bool includePuppeteer=false,
        bool includeDataContext=false)
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
                {
                    sc.AddSingleton<ISettingsService, SettingsService>();
                    sc.AddSingleton<ISecureSettingsService, TestSettingsService>();
                }               
                #pragma warning restore CA1416

               if (includeMediator)
                   sc.AddMediatR(typeof(LoginQuery));

               if (includePuppeteer)
               {
                   sc.AddSingleton<BrowserProvider>();

                   sc.AddSingleton<IWebSiteNavigator, LinkedInNavigator>();
                   sc.AddSingleton<IWebSiteNavigator, GoogleJobsNavigator>();
                   sc.AddSingleton<IWebSiteNavigator, IndeedNavigator>();
                   sc.AddSingleton<IWebSiteNavigator, SimplyHiredNavigator>();
                   sc.AddSingleton<IWebSiteNavigator, DiceNavigator>();
                   sc.AddSingleton<IWebSiteNavigator, MonsterNavigator>();

                   sc.AddSingleton<IJobPostingExtractor, LinkedInJobPostingExtractor>();
                   sc.AddSingleton<IJobPostingExtractor, GoogleJobsExtractor>();
                   sc.AddSingleton<IJobPostingExtractor, DiceJobPostingExtractor>();
                   sc.AddSingleton<IJobPostingExtractor, IndeedJobPostingExtractor>();
                   sc.AddSingleton<IJobPostingExtractor, MonsterJobPostingExtractor>();
                   sc.AddSingleton<IJobPostingExtractor, SimplyHiredJobPostingExtractor>();
               }

               if (includeDataContext)
               {
                   if(File.Exists("test.db"))
                   {
                       File.Delete("test.db");
                   }
                   sc.AddSingleton<LocalDataContextProvider>();
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

    public LocalDataContextProvider LocalDataContextProvider => _serviceProvider.GetRequiredService<LocalDataContextProvider>();

}
