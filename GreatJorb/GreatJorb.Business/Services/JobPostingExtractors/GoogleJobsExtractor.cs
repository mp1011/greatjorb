namespace GreatJorb.Business.Services.JobPostingExtractors;

public class GoogleJobsExtractor : IJobPostingExtractor
{
    private readonly IMediator _mediator;

    public GoogleJobsExtractor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string WebsiteName => "Google Jobs";

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int pageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        List<JobPosting> jobs = new();

        var jobHeaders = await page.QuerySelectorAllAsync("li.iFjolb");

        jobHeaders = jobHeaders
            .Skip((pageNumber - 1) * 10)
            .ToArray();

        if (PageSize.HasValue)
        {
            jobHeaders = jobHeaders
                .Take(PageSize.Value)
                .ToArray();
        }

        foreach(var jobHeader in jobHeaders)
        {
            jobs.Add(await ExtractJob(jobHeader, site, page, cancellationToken));
        }

        return jobs.ToArray();
    }

    private async Task<JobPosting> ExtractJob(IElementHandle element, WebSite site, IPage page, CancellationToken cancellationToken)
    {
        var lines = await element
            .ExtractTextFromLeafNodes("div,span");
       
        var jobPosting =  new JobPosting
        {
            Title = lines[0],
            Company = lines[1],
            Location = lines[2],
        };

        foreach(var line in lines)
        {
            await _mediator.Send(new SetPropertiesFromTextCommand(jobPosting, line));
        }

        await element.ClickAsync();

        var jobDescriptionHeading = await page.GetElementByInnerText("div", "Job Description", cancellationToken);
        if (jobDescriptionHeading == null)
            return jobPosting;

        jobPosting.DescriptionHtml = await jobDescriptionHeading
            .NextElementAsync(page, cancellationToken)
            .GetInnerHTML();

        jobPosting.Uri = new Uri(page.Url);
        jobPosting.StorageKey = page.Url;

        await _mediator.Publish(new JobPostingRead(jobPosting, site, FromCache: false));

        return jobPosting;
    }


}
