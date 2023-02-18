namespace GreatJorb.Business.Services.JobPostingExtractors;

public class SimplyHiredJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public SimplyHiredJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string WebsiteName => Site.SimplyHired.GetDisplayName();

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        var cards = await page.QuerySelectorAllAsync("#job-list li");

        List<string> urls = new();
        foreach(var card in cards)
        {
            var url = await card.QuerySelectorAsync("a").GetAttribute("href");

            if (!url.IsNullOrEmpty() && url.StartsWith("/job"))
                urls.Add(url);

            if (PageSize.HasValue && urls.Count == PageSize)
                break;
        }

        List<JobPosting> jobs = new();
        foreach(var url in urls)
        {
            await page.GoToAsync(url);
            jobs.Add(await ExtractJob(page, cancellationToken));
        }

        return jobs.ToArray();
    }

    private async Task<JobPosting> ExtractJob(IPage page, CancellationToken cancellationToken)
    {
        var job = new JobPosting();

        job.Uri = new Uri(page.Url);
        job.StorageKey = page.Url;

        job.Title = await page
            .QuerySelectorAsync("h2[data-testid='viewJobTitle']")
            .GetInnerText();

        job.Company = await page
            .QuerySelectorAsync("span[data-testid='viewJobCompanyName']")
            .GetInnerText();

        var rightPanel = await page.QuerySelectorAsync("aside");

        var textList = await rightPanel.ExtractTextFromLeafNodes("span");

        foreach(var text in textList)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, text));
        }

        job.DescriptionHtml = await page
            .QuerySelectorAsync("div[data-testid='viewJobBodyJobFullDescriptionContent']")
            .GetInnerHTML();

        return job;
    }
}
