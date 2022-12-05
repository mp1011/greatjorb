namespace GreatJorb.Business.Services.WebSiteNavigators;

public class LinkedInNavigator : IWebSiteNavigator
{
    public string WebsiteName => "LinkedIn";

    public async Task<IElementHandle?> GetLoginButton(IPage page) =>
        await page.QuerySelectorAsync(".sign-in-form__submit-button");

    public async Task<IElementHandle?> GetLoginElement(IPage page) =>
        await page.GetElementLabelledBy("Email or phone number");

    public async Task<IElementHandle?> GetPasswordElement(IPage page) =>
        await page.GetElementLabelledBy("Password");

    public async Task<IPage> GotoJobsListPage(IPage page, string query)
    {
        await page.GoToAsync("https://www.linkedin.com/jobs/search");

        await page
            .GetElementLabelledBy("Search by title, skill, or company")
            .SetText(page, query, clearExistingText:false, pressEnter: true);

        return page;
    }
}
