namespace GreatJorb.UI.Components.Settings;

public partial class CredentialsPanel
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ISecureSettingsService SettingsService { get; set; }

    public WebSiteCredentials[] Sites { get; set; } = Array.Empty<WebSiteCredentials>();

    public class WebSiteCredentials
    {
        public WebSite Site { get; }

        public WebSiteCredentials(WebSite site, string userName, string password)
        {
            Site = site;
            _userName = userName;
            _password = password;
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if(_userName != value)
                {
                    HasChanges = true;
                    _userName = value;
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    HasChanges = true;
                    _password = value;
                }
            }
        }

        public bool HasChanges { get; set; }
    }

    protected override async Task OnInitializedAsync()
    {
        var sites = (await Mediator.Send(new GetSitesQuery()))
            .Where(p => p.RequiresCredentials)
            .ToArray();

        List<WebSiteCredentials> creds = new();
        foreach(var site in sites)
        {
            creds.Add(await GetCredentials(site));
        }

        Sites = creds.ToArray();
    }

    private async Task<WebSiteCredentials> GetCredentials(WebSite site)
    {
        var realPwd = await SettingsService.GetSitePassword(site);

        return new WebSiteCredentials(site,
            await SettingsService.GetSiteUserName(site),
            realPwd.IsNullOrEmpty() ? "" : "".PadRight(realPwd.Length, '*'));
    }

    public void SaveCredentials(WebSiteCredentials credentials)
    {
        SettingsService.SetSiteUserName(credentials.Site, credentials.UserName);

        if(!credentials.Password.Replace("*","").IsNullOrEmpty())
        {
            SettingsService.SetSitePassword(credentials.Site, credentials.Password);
        }

        credentials.HasChanges = false;
    }

}
