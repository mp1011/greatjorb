namespace GreatJorb.Business.Services.JobPostingExtractors;

public class MonsterJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => Site.Monster.ToString();

    public MonsterJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        var cards = await page.QuerySelectorAllAsync("div[class*='JobCardWrap']");

        List<string> jobUrls = new();
        foreach(var card in cards)
        {
            jobUrls.Add(await card.QuerySelectorAsync("a").GetAttribute("href"));
        }

        throw new NotImplementedException();
    }

    public async Task<JobPosting> ExtractJob(IPage page, string url, CancellationToken cancellationToken)
    {
        var jobContainer = await page.QuerySelectorAsync("div[class*='JobContainer']");

        var job = new JobPosting();
        job.Uri = new Uri(url);
        job.StorageKey = url;

        job.Title = await jobContainer
            .QuerySelectorAsync(".JobViewTitle")
            .GetInnerText();

        job.Company = await jobContainer
            .QuerySelectorAsync("h2[class*='JobViewHeaderCompany']")
            .GetInnerText();

        var lines = await jobContainer.ExtractTextFromLeafNodes("div,span,h1", delimiter: ',');

        foreach(var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(job, line));
        }

        job.DescriptionHtml = await jobContainer
            .QuerySelectorAsync("div[class*='DescriptionContainer']")
            .GetInnerHTML();

        job.Location = await jobContainer
            .QuerySelectorAsync("div[data-test-id='svx-jobview-location']")
            .GetInnerText();

        job.Location = job.Location.Split('\n').Last();


        return job;
    }
}
