namespace GreatJorb.Business.Services.JobPostingExtractors;

public class LinkedInJobPostingExtractor : IJobPostingExtractor
{
    public string WebsiteName => "LinkedIn";


    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page)
    {
        var jobCards = await page.QuerySelectorAllAsync(".job-card-container");

        List<JobPosting> postingHeaders = new();

        foreach(var jobCard in jobCards)
        {
            postingHeaders.Add(await ExtractPostingHeaders(jobCard));
        }

        List<JobPosting> postingDetails = new();
        foreach(var header in postingHeaders)
        {
            postingDetails.Add(await ExtractPostingDetails(page, header));
        }

        return postingDetails.ToArray();
    }

    private async Task<JobPosting> ExtractPostingHeaders(IElementHandle jobCard)
    {
        return new JobPosting
        {
            Url = await jobCard
                .QuerySelectorAsync("a.job-card-container__link")
                .GetAttribute("href"),
            Title = await jobCard.GetTextAsync(".job-card-list__title"),
            Company = await jobCard.GetTextAsync(".job-card-container__company-name"),
            WorkplaceType = await jobCard
                .GetTextAsync(".job-card-container__metadata-item--workplace-type")
                .ParseWorkplaceType(),
            JobType = JobType.Unknown,
            SalaryType = SalaryType.Unknown
        };
    }

    private async Task<JobPosting> ExtractPostingDetails(IPage page, JobPosting posting)
    {
        await page.GoToAsync($"https://linkedin.com/{posting.Url}");

        var metaData = await page
            .QuerySelectorAllAsync(".job-card-container__metadata-item")
            .GetInnerTextAsync();

        var insights = await page
            .QuerySelectorAllAsync(".jobs-unified-top-card__job-insight")
            .GetInnerTextAsync();

        var description = await page
            .QuerySelectorAsync(".jobs-description-content")
            .GetInnerHTML();

        posting.DescriptionHtml = description;

        posting.MiscProperties = metaData
            .Union(insights)
            .ToArray();

        return posting;       
    }
}
