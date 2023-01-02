namespace GreatJorb.Business.Services.JobPostingExtractors;

public class GoogleJobsExtractor : IJobPostingExtractor
{
    public string WebsiteName => "Google Jobs";

    public async Task<JobPosting[]> ExtractJobsFromPage(IPage page, int pageNumber, WebSite site, CancellationToken cancellationToken, JobFilter filter, int? PageSize = null)
    {
        List<JobPosting> jobs = new();

        var jobHeaders = await page.QuerySelectorAllAsync("li.iFjolb");

        jobHeaders = jobHeaders
            .Skip((pageNumber - 1) * 10)
            .ToArray();

        foreach(var jobHeader in jobHeaders)
        {
            jobs.Add(await ExtractJob(jobHeader, page, cancellationToken));
        }

        return jobs.ToArray();
    }

    private async Task<JobPosting> ExtractJob(IElementHandle element, IPage page, CancellationToken cancellationToken)
    {
        var lines = (await element
                            .GetInnerText() ?? "")
                            .Split("\n", StringSplitOptions.RemoveEmptyEntries);

        var jobPosting =  new JobPosting
        {
            Title = lines[0],
            Company = lines[1],
            Location = lines[2],
        };

        foreach(var line in lines)
        {
            TrySetPropertyByText(jobPosting, line);
        }

        await element.ClickAsync();

        var jobDescriptionHeading = await page.GetElementByInnerText("div", "Job Description", cancellationToken);
        if (jobDescriptionHeading == null)
            return jobPosting;

        jobPosting.DescriptionHtml = await jobDescriptionHeading
            .NextElementAsync(page, cancellationToken)
            .GetInnerHTML();
       
        return jobPosting;
    }

    private void TrySetPropertyByText(JobPosting header, string text)
    {
        //		[5]	"100K–110K a year"	string

        var maybeJobType = text.TryParseEnumAdvanced<JobType>(JobType.Unknown);
        if(maybeJobType != JobType.Unknown)
        {
            header.JobType = maybeJobType;
            return;
        }

        var maybeSalary = Regex.Match(text, @"(\d+)K–(\d+)K(.*)");

        if(maybeSalary.Success)
        {
            int num1 = maybeSalary.Groups[1].Value.TryParseInt(0);
            int num2 = maybeSalary.Groups[2].Value.TryParseInt(0);
            string type = maybeSalary.Groups[3].Value;

            if (type.Contains("year"))
                header.SalaryType = SalaryType.Annual;
            else
                throw new NotImplementedException();

            if (num1 > 0)
                header.SalaryMin = num1 * 1000;
            if (num2 > 0)
                header.SalaryMax = num2 * 1000;

            return;
        }

        //		[6]	"Full-time"	string

    }

}
