using GreatJorb.Business.Exceptions;

namespace GreatJorb.Business.Services.WebSiteNavigators;

public class LinkedInNavigator : IWebSiteNavigator
{
    private readonly IMediator _mediator;

    public LinkedInNavigator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string WebsiteName => "LinkedIn";

    public async Task<IElementHandle?> GetLoginButton(IPage page) =>
        await page.QuerySelectorAsync(".sign-in-form__submit-button");

    public async Task<IElementHandle?> GetLoginElement(IPage page) =>
        await page.GetElementLabelledBy("Email or phone number");

    public async Task<IElementHandle?> GetPasswordElement(IPage page) =>
        await page.GetElementLabelledBy("Password");

    public async Task<IPage> GotoJobsListPage(IPage page, string query, int pageNumber)
    {
        await page.GoToAsync($"https://www.linkedin.com/jobs/search/?keywords={query.UrlEncode()}");
        //await page.GoToAsync("https://www.linkedin.com/jobs/search");
        //await page
        //    .GetElementLabelledBy("Search by title, skill, or company")
        //    .SetText(page, query, clearExistingText: false, pressEnter: true);

        if (pageNumber > 1)
        {
            var pager = await page.WaitForSelectorAsync("ul.artdeco-pagination__pages");
            var pagerElements = await pager.QuerySelectorAllAsync("li.artdeco-pagination__indicator");

            foreach (var element in pagerElements)
            {
                var elementPageNum = await element.TryGetNumberAsync();
                if (elementPageNum.HasValue && elementPageNum.Value == pageNumber)
                {
                    await element.ClickAsync();
                    return page;
                }
            }
            throw new PageNumberNotFoundException();
        }

        return page;
    }

    public async Task WaitUntilLoggedIn(IPage page, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await page.WaitForSelectorAsync("img.global-nav__me-photo",
                new WaitForSelectorOptions { Timeout = 5000 })
                .DefaultIfError();

            await _mediator.Publish(new BrowserPageChanged(page));

            if (result != null)
                return;
        }
    }
}
