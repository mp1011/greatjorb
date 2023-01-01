using PuppeteerSharp;
using System.IO;

namespace GreatJorb.Tests;

[SupportedOSPlatform("windows")]
public class TestSettingsService : ISettingsService
{
    private readonly SettingsService _settingsService;
    private readonly IMediator _mediator;

    public TestSettingsService(SettingsService settingsService, IMediator mediator)
    {
        _settingsService = settingsService;
        _mediator = mediator;
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

    private string PromptUser(string prompt)
    {
        var task = Task.Run(async () => await PromptUserAsync(prompt));
        return task.Result;
    }

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

    public string GetSitePassword(WebSite site)
    {
        var pwd = _settingsService.GetSitePassword(site);
        if(pwd.IsNullOrEmpty())
        {
            pwd = PromptUser("Enter Password for " + site.Name);
            _settingsService.SetSitePassword(site, pwd);
        }

        return pwd;
    }

    public string GetSiteUserName(WebSite site)
    {
        var user = _settingsService.GetSiteUserName(site);
        if (user.IsNullOrEmpty())
        {
            user = PromptUser("Enter Username for " + site.Name);
            _settingsService.SetSiteUserName(site, user);
        }

        return user;
    }

    public void SetSitePassword(WebSite site, string value) => _settingsService.SetSitePassword(site, value);

    public void SetSiteUserName(WebSite site, string value) => _settingsService.SetSiteUserName(site, value);
}
