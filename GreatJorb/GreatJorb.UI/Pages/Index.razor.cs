namespace GreatJorb.UI.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ICourier Courier { get; set; }

    public List<JobPosting> Postings { get; } = new();
    public List<JobPosting> NonMatchingPostings { get; } = new();

    public List<BrowserPageChanged> Notifications { get; } = new();

    private JobFilter _currentFilter = new JobFilter();
    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnInitialized()
    {
        Courier.Subscribe<BrowserPageChanged>(OnNavigation);
        Courier.Subscribe<JobPostingRead>(OnJobPostingRead);

        Postings.Add(new JobPosting("https://www.google.com")
        {
            Company = "Test Company",
            JobLevel = JobLevel.SeniorLevel,
            JobType = JobType.FullTime,
            SalaryMin = 100000,
            SalaryMax = 150000,
            Title = "Test Job",
            SalaryType = SalaryType.Annual,
            Location = "New York, NY",
            WorkplaceType = WorkplaceType.Remote
        });
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
        NonMatchingPostings.Clear();

        WebSite site = new WebSite("LinkedIn", "https://www.linkedin.com/");
        _currentFilter = filter;
        var loginResult = await Mediator.Send(new LoginQuery(site));

        var cacheResult = await Mediator.Send(new SearchJobsFromCacheQuery(site, filter));
        Postings.AddRange(cacheResult.MatchesFilter);
        NonMatchingPostings.AddRange(cacheResult.DoesNotMatchFilter);

        StateHasChanged();
        
        await Mediator
            .Send(new SearchJobsQuery(loginResult.Data, filter, 5), _cancellationTokenSource.Token)
            .IgnoreCancellationException();
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(async () =>
        {
            if (!Postings.Any(p => p.StorageKey == notification.Job.StorageKey))
            {
                if (_currentFilter.IsMatch(notification.Job))
                    Postings.Add(notification.Job);
                else
                    NonMatchingPostings.Add(notification.Job);

                await Mediator.Send(new AddJobResultToCacheCommand(notification.Site, notification.Job));

                StateHasChanged();
            }
        });
    }

    public Task OnNavigation(BrowserPageChanged notification)
    {
        Notifications.Add(notification);
        StateHasChanged();

        return Task.CompletedTask;
    }
}
