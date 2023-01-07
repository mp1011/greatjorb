namespace GreatJorb.UI.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ICourier Courier { get; set; }

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

    public async Task PerformSearch(JobFilter filter)
    {
        CurrentFilter = filter;

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();

        Postings.Clear();

        await Mediator.Send(new AddFilterToCacheCommand(filter));
        await Mediator.Send(new SearchJobsFromMultipleSitesQuery(filter, 10), _cancellationTokenSource.Token);

        StateHasChanged();
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(async () =>
        {
            if (Postings.Any(p => p.Job.StorageKey == notification.Job.StorageKey))
                return;

            var keywordLines = await Mediator.Send(new ExtractKeywordLinesQuery(CurrentFilter.Query, notification.Job.DescriptionHtml));

            Postings.Add(new JobPostingSearchResult(
                notification.Job,
                notification.Site,                 
                await Mediator.Send(new MatchJobFilterQuery(notification.Job, keywordLines.Any(), CurrentFilter)),
                keywordLines));

            Postings.Sort();

            if(!notification.FromCache)
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
