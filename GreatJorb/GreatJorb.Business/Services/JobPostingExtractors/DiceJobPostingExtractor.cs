namespace GreatJorb.Business.Services.JobPostingExtractors;

public class DiceJobPostingExtractor : IJobPostingExtractor
{
    public string WebsiteName => Site.Dice.ToString();

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int PageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        var cards = await page.QuerySelectorAllSafeAsync("dhi-search-card", cancellationToken);

        List<string> detailUrls = new();
        foreach(var card in cards)
        {
            var urlElement = card.QuerySelectorAsync("a.card-title-link");
            if (urlElement != null)
                detailUrls.Add(await urlElement.GetAttribute("href"));
        }

        List<JobPosting> postings = new();

        foreach(var detailUrl in detailUrls)
        {
            await page.GoToAsync(detailUrl);
            await page.WaitForDOMIdle(cancellationToken);
            postings.Add(await ExtractJobDetail(page, cancellationToken));
        }

        return postings.ToArray();
    }

    public async Task<JobPosting> ExtractJobDetail(IPage page, CancellationToken cancellation)
    {
        var job = new JobPosting();

        job.Uri = new Uri(page.Url);
        job.StorageKey = job.Uri.Host + job.Uri.LocalPath;
        job.Title = await page
            .QuerySelectorAsync("h1[data-cy='jobTitle']")
            .GetInnerText();

        return job;
    }
}
