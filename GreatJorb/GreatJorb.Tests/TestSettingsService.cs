using GreatJorb.Business.Services.Settings;
using PuppeteerSharp;

namespace GreatJorb.Tests;

[SupportedOSPlatform("windows")]
public class TestSettingsService : ISecureSettingsService
{
    private readonly SettingsService _settingsService;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public TestSettingsService(SettingsService settingsService, IConfiguration configuration, IMediator mediator)
    {
        _settingsService = settingsService;
        _mediator = mediator;
        _configuration = configuration;
    }

    public string LocalStoragePath
    {
        get => _settingsService.LocalStoragePath;
        set => _settingsService.LocalStoragePath = value;
    }

    public int MaxNavigationRetries => _settingsService.MaxNavigationRetries;

    public TimeSpan WaitAfterFailedNavigate => _settingsService.WaitAfterFailedNavigate;

    public TimeSpan MinTimeBetweenRequests => _settingsService.MinTimeBetweenRequests;

    public bool UseHeadlessBrowser => _settingsService.UseHeadlessBrowser;

    private async Task<string> PromptUserAsync(string prompt)
    {
        if (File.Exists("prompt.html"))
            File.Delete("prompt.html");

        File.WriteAllText("prompt.html",
                            @"<html>
                                <body>
                                    <h1>" + prompt + @"</h1>
                                    <input type='text' id='prompt' />
                                    <button id='enter' onclick='submitClicked()'>Submit</button>

                                    <script type='text/javascript'>

                                        var promptValue='';

                                        function submitClicked()
                                        {
                                            promptValue = document.getElementById('prompt').value;
                                        }

                                    </script>
                                </body>
                             </html>");

        FileInfo f = new FileInfo("prompt.html");

        IPage page = await _mediator.Send(new BrowseToPageQuery(@$"file://{f.FullName}"));
        var inputElement = await page.WaitForSelectorAsync("#prompt");

        string? text = null;

        while (text!.IsNullOrEmpty())
        {
            text = await page.EvaluateFunctionAsync<string>("e=>promptValue");
            
            if(text.IsNullOrEmpty())
                await Task.Delay(1000);
        }

        await page.CloseAsync();

        return text ?? "";
    }

    public async Task<string?> GetSitePassword(WebSite site)
    {
        var pwd = await GetSecureText($"{site.Name}.Password");
        if (pwd.IsNullOrEmpty())
        {
            pwd = await PromptUserAsync("Enter Password for " + site.Name);
            await SetSitePassword(site, pwd);
        }

        return pwd;
    }

    public async Task<string?> GetSiteUserName(WebSite site)
    {
        var user = await GetSecureText($"{site.Name}.UserName");
        if (user.IsNullOrEmpty())
        {
            user = await PromptUserAsync("Enter Username for " + site.Name);
            await SetSiteUserName(site, user);
        }

        return user;
    }

    public async Task SetSitePassword(WebSite site, string value) => await SetSecureText($"{site.Name}.Password", value);

    public async Task SetSiteUserName(WebSite site, string value) => await SetSecureText($"{site.Name}.UserName", value);


    private Task<string> GetSecureText(string key)
    {
        if (key == null)
            return string.Empty.AsTaskResult();

        try
        {
            return SecureSettingsHelper
                .ReadSecureText(key, _configuration[key]!)
                .AsTaskResult();
        }
        catch
        {
            return string.Empty.AsTaskResult();
        }
    }

    private Task SetSecureText(string key, string value)
    {
        var encryptedText = SecureSettingsHelper.EncryptText(key, value);

#if DEBUG
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo();
            psi.WorkingDirectory = Environment.CurrentDirectory.Substring(0, Environment.CurrentDirectory.IndexOf("\\bin"));
            psi.FileName = "dotnet";
            psi.Arguments = $"user-secrets set \"{key}\" \"{encryptedText}\"";

            System.Diagnostics.Process
                .Start(psi)
                !.WaitForExit();
        }
        catch 
        {
            throw;
        }
#endif

        return Task.CompletedTask;
    }
}
