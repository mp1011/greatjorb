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
#pragma warning restore CA1416

		builder.Services.AddMediatR(typeof(LoginQuery));
		builder.Services.AddCourier(typeof(LoginQuery).Assembly);

		builder.Services.AddSingleton<BrowserProvider>();
		builder.Services.AddSingleton<IWebSiteNavigator, LinkedInNavigator>();
		builder.Services.AddSingleton<IWebSiteNavigator, GoogleJobsNavigator>();


		builder.Services.AddSingleton<IJobPostingExtractor, LinkedInJobPostingExtractor>();
		builder.Services.AddSingleton<IJobPostingExtractor, GoogleJobsExtractor>();

		builder.Services.AddSingleton<LocalDataContextProvider>();

		var config = new ConfigurationBuilder()
				.AddUserSecrets(typeof(MauiProgram).Assembly)
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
