namespace GreatJorb.Business.Features;

public record LoginQuery(WebSite Site) : IRequest<Result<WebPage>>
{

    class Handler : IRequestHandler<LoginQuery, Result<WebPage>>
    {
        private readonly IMediator _mediator;
        private readonly ISettingsService _settingsService;

        public Handler(IMediator mediator, ISettingsService settingsService)
        {
            _mediator = mediator;
            _settingsService = settingsService;
        }

        public async Task<Result<WebPage>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            IPage page = await _mediator.Send(new BrowseToPageQuery(request.Site.Url));

            IWebSiteNavigator? navigator = await _mediator.Send(new GetNavigatorQuery(request.Site));
            if(navigator == null)
                return new Result<WebPage>(false, new WebPage(request.Site, null));

            string loginPageUrl = page.Url;

            await navigator
                .GetLoginElement(page)
                .SetText(page,_settingsService.GetSiteUserName(request.Site));

            await navigator
                .GetPasswordElement(page)
                .SetText(page, _settingsService.GetSitePassword(request.Site));

            await navigator
                .GetLoginButton(page)
                .ClickAsync();

            await page.WaitForNavigationFromAsync(loginPageUrl);

            return new Result<WebPage>(page.Url != loginPageUrl, new WebPage(request.Site,page));
        }
    }
}
