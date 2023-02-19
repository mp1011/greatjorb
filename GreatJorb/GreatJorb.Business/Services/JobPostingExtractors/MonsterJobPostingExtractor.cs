namespace GreatJorb.Business.Services.JobPostingExtractors;

public class MonsterJobPostingExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public string WebsiteName => Site.Monster.ToString();

    public MonsterJobPostingExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }


    public string GetStorageKeyFromUrl(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<JobPosting?> ExtractNextJob(IPage page, HashSet<string> knownJobs, CancellationToken cancellationToken)
    {
        var cards = await page.QuerySelectorAllAsync("div[class*='JobCardWrap']");

        foreach(var card in cards)
        {
            var url = await card.QuerySelectorAsync("a").GetAttribute("href");
            if (knownJobs.Contains(GetStorageKeyFromUrl(url)))
                continue;

            await page.GoToAsync("https:" + url);
            return await ExtractJob(page, url, cancellationToken);
        }

        return null;
    }

    public async Task<JobPosting> ExtractJob(IPage page, string url, CancellationToken cancellationToken)
    {
        var jobContainer = await page.QuerySelectorAsync("div[class*='jobview-container']");

        var job = new JobPosting();
        job.Uri = new Uri(url);
        job.StorageKey = GetStorageKeyFromUrl(page.Url); 

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
