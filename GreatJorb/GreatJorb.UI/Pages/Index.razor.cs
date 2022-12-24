namespace GreatJorb.UI.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ICourier Courier { get; set; }

    public List<JobPostingSearchResult> Postings { get; } = new();

    public List<BrowserPageChanged> Notifications { get; } = new();

    private JobFilter _currentFilter = new JobFilter();
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

    public async Task PerformSearch(JobFilter filter)
    {
        if(_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();

        Postings.Clear();

        WebSite site = new WebSite("LinkedIn", "https://www.linkedin.com/");
        _currentFilter = filter;
        var loginResult = await Mediator.Send(new LoginQuery(site));

        var cacheResult = await Mediator.Send(new SearchJobsFromCacheQuery(site, filter));
        Postings.AddRange(cacheResult);

        StateHasChanged();
        
        await Mediator
            .Send(new SearchJobsQuery(loginResult.Data, filter, 5), _cancellationTokenSource.Token)
            .IgnoreCancellationException();
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(async () =>
        {
            if (Postings.Any(p => p.Job.StorageKey == notification.Job.StorageKey))
                return;

            var keywordLines = await Mediator.Send(new ExtractKeywordLinesQuery(_currentFilter.Query, notification.Job.DescriptionHtml));

            Postings.Add(new JobPostingSearchResult(
                notification.Job,
                await Mediator.Send(new MatchJobFilterQuery(notification.Job, keywordLines.Any(), _currentFilter)),
                keywordLines));

            await Mediator.Send(new AddJobResultToCacheCommand(notification.Site, notification.Job));

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
