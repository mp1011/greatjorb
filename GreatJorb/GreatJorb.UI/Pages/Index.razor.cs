namespace GreatJorb.UI.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ICourier Courier { get; set; }

    [Inject]
    public ISecureSettingsService SecureSettingsService { get; set; }

    [Inject]
    public IJSRuntime JsRuntime { get; set; }

    public List<JobPostingSearchResult> Postings { get; } = new();

    public List<BrowserPageChanged> Notifications { get; } = new();

    public JobFilter CurrentFilter { get; set; } = new JobFilter();

    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnInitialized()
    {
        Courier.Subscribe<BrowserPageChanged>(OnNavigation);
        Courier.Subscribe<JobPostingRead>(OnJobPostingRead);
    }

    public void Dispose()
    {
        Courier.UnSubscribe<BrowserPageChanged>(OnNavigation);
        Courier.UnSubscribe<JobPostingRead>(OnJobPostingRead);
        Mediator.Send(new DisposeBrowserCommand());
    }

    protected override async Task OnInitializedAsync()
    {
        var filterHistory = await Mediator.Send(new LoadFiltersFromCacheQuery());
        if (filterHistory.Any())
        {
            CurrentFilter = filterHistory
                .OrderByDescending(p => p.Date)
                .First()
                .Filter;

            StateHasChanged();
        }
    }

    private async Task PromptForCredentialsIfNeeded()
    {
        var sites = await Mediator.Send(new GetSitesQuery(CurrentFilter.Sites));

        foreach(var site in sites)
        {
            if (!site.RequiresCredentials)
                continue;

            var userName = await SecureSettingsService.GetSiteUserName(site);
            var pwd = await SecureSettingsService.GetSitePassword(site);
            if(userName.IsNullOrEmpty() || pwd.IsNullOrEmpty())
            {
                await JsRuntime.ShowModal("CredentialsModal");
            }
        }
    }

    public async Task PerformSearch(JobFilter filter)
    {      
        CurrentFilter = filter;

        //temporary until ui is made
        CurrentFilter.WatchWords = new WatchWord[]
        {
            new WatchWord(".net", isGood:true),
            new WatchWord("offshore", isGood:false),
            new WatchWord("travel", isGood:null),
            new WatchWord("ny", isGood:null),
            new WatchWord("new york", isGood:null),
            new WatchWord("states", isGood:null),
        };

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        await PromptForCredentialsIfNeeded();

        _cancellationTokenSource = new CancellationTokenSource();

        Postings.Clear();

        await Mediator.Send(new AddFilterToCacheCommand(filter));
        await Mediator.Send(new SearchJobsFromMultipleSitesQuery(filter), _cancellationTokenSource.Token);

        StateHasChanged();
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(async () =>
        {
            Postings.RemoveAll(p => p.Job.StorageKey == notification.JobSearchResult.Job.StorageKey);

            Postings.Add(notification.JobSearchResult);
             
            Postings.Sort();

            if(!notification.FromCache)
                await Mediator.Send(new AddJobResultToCacheCommand(notification.JobSearchResult.Site, notification.JobSearchResult.Job));

            StateHasChanged();            
        });
    }

    public Task OnNavigation(BrowserPageChanged notification)
    {
        Notifications.Add(notification);
        StateHasChanged();

        return Task.CompletedTask;
    }
}
