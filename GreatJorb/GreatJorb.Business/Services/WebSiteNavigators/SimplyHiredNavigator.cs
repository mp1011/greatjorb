namespace GreatJorb.Business.Services.WebSiteNavigators;

public class SimplyHiredNavigator : IWebSiteNavigator
{
    private ISettingsService _settings;

    public SimplyHiredNavigator(ISettingsService settings)
    {
        _settings = settings;
    }

    public Site Website => Site.SimplyHired;
    
    public async Task<IPage> ApplyFilters(IPage page, JobFilter filter, CancellationToken cancellationToken)
    {
        if(filter.WorkplaceTypeFilter != WorkplaceType.Unknown)
        {
            string? locationQuery = null;

            if (filter.WorkplaceTypeFilter.HasFlag(WorkplaceType.Remote))
                locationQuery = "Remote";
            else
                locationQuery = _settings.OnsiteLocation;

            if(locationQuery != null)
            {
                await page.ChangeQuerystring("l", locationQuery, cancellationToken);
            }
        }

        if(filter.JobTypeFilter != JobType.Unknown)
        {
            await page
                .GetElementLabelledBy("Job Type Filter", cancellationToken)
                .ClickAsync();

            await Task.Delay(500);

            var searchText = filter.JobTypeFilter switch
            {
                JobType.FullTime => "Full-time",
                JobType.Contract => "Contract",
                JobType.PartTime => "Part-time",
                _ => "All Job Types"
            };

            await page
                .GetElementByInnerText("a", searchText, cancellationToken)
                .ClickAsync();
        }

        if(filter.Salary.HasValue)
        {
            await page
                .GetElementLabelledBy("Minimum Salary Filter", cancellationToken)
                .ClickAsync();

            await Task.Delay(500);

            var salaryOptions = await page.QuerySelectorAllAsync("#Filters-dropdownBody-MinimumPay a");

            foreach(var option in salaryOptions)
            {
                var salary = (await option.GetInnerText()).TryParseCurrency();
                if(salary != null && salary >= filter.Salary.Value)
                {
                    await option.ClickAsync();
                    break;
                }
            }
        }

        return page;
    }

    public Task<IElementHandle?> GetLoginButton(IPage page, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<IElementHandle?> GetLoginElement(IPage page, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task<IElementHandle?> GetPasswordElement(IPage page, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public async Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber, CancellationToken cancellationToken)
    {
        await page.GoToAsync($"https://www.simplyhired.com/search?q={query.UrlEncode()}");
        return page; 
    }

    public Task<bool> IsLoginRequired(IPage page, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }

    public Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
