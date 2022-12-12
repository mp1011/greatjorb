namespace GreatJorb.UI.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public ICourier Courier { get; set; }

    public List<JobPosting> Postings { get; } = new();

    public List<BrowserPageChanged> Notifications { get; } = new();

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

    public async Task DoMagicStuff()
    {
        var loginResult = await Mediator.Send(new LoginQuery(new WebSite("LinkedIn", "https://www.linkedin.com/")));

        await Mediator.Send(new SearchJobsQuery(loginResult.Data, "c#", 5));
    }

    public async Task OnJobPostingRead(JobPostingRead notification)
    {
        await InvokeAsync(() =>
        {
            if (!Postings.Any(p => p.Url == notification.Job.Url))
            {
                Postings.Add(notification.Job);
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
