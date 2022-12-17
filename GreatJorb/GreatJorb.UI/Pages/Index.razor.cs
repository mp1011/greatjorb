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

    protected override void OnInitialized()
    {
        Courier.Subscribe<BrowserPageChanged>(OnNavigation);
        Courier.Subscribe<JobPostingRead>(OnJobPostingRead);

        Postings.Add(new JobPosting
        {
            Company = "Test Company",
            JobLevel = JobLevel.SeniorLevel,
            JobType = JobType.FullTime,
            SalaryMin = 100000,
            SalaryMax = 150000,
            Title = "Test Job",
            Url = "https://www.google.com",
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
        _currentFilter = filter;
        var loginResult = await Mediator.Send(new LoginQuery(new WebSite("LinkedIn", "https://www.linkedin.com/")));
        await Mediator.Send(new SearchJobsQuery(loginResult.Data, filter, 5));
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(() =>
        {
            if (!Postings.Any(p => p.Url == notification.Job.Url))
            {
                if (_currentFilter.IsMatch(notification.Job))
                    Postings.Add(notification.Job);
                else
                    NonMatchingPostings.Add(notification.Job);

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
