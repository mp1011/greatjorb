namespace GreatJorb.UI;

public static class MauiProgram
{
	private static MauiApp _app;

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			})
			.ConfigureLifecycleEvents(e =>
            {
#if WINDOWS
				e.AddWindows(w =>
					w.OnClosed((window, args) => OnAppClosed()));
#endif 
            });

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddBlazorWebViewDeveloperTools();

#pragma warning disable CA1416
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
		builder.Services.AddSingleton<ISecureSettingsService, LiteDbSettingsService>();
#pragma warning restore CA1416

		builder.Services.AddMediatR(typeof(LoginQuery));
		builder.Services.AddCourier(typeof(LoginQuery).Assembly);

		builder.Services.AddSingleton<BrowserProvider>();
		builder.Services.AddSingleton<IWebSiteNavigator, LinkedInNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, GoogleJobsNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, IndeedNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, DiceNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, MonsterNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, SimplyHiredNavigator>();

		builder.Services.AddSingleton<IJobPostingExtractor, LinkedInJobPostingExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, GoogleJobsExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, DiceJobPostingExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, IndeedJobPostingExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, MonsterJobPostingExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, SimplyHiredJobPostingExtractor>();

		builder.Services.AddSingleton<LocalDataContextProvider>();

		builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlerBehavior<,>));

		var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")				
				.Build();

		builder.Configuration.AddConfiguration(config);

		return _app = builder.Build();
	}

	private static void OnAppClosed()
    {
		var mediator = _app.Services.GetRequiredService<IMediator>();

		Task.Run(() => mediator.Send(new DisposeBrowserCommand()));
    }
}
