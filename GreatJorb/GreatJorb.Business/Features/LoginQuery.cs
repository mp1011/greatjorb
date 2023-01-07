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

            if (!await navigator.IsLoginRequired(page))
            {
                return new Result<WebPage>(true, new WebPage(request.Site, page));
            }

            string userName = _settingsService.GetSiteUserName(request.Site);
            string password = _settingsService.GetSitePassword(request.Site);

            await navigator
                .GetLoginElement(page, cancellationToken)
                .SetText(page, userName);

            await navigator
                .GetPasswordElement(page, cancellationToken)
                .SetText(page, password);

            await navigator
                .GetLoginButton(page, cancellationToken)
                .ClickAsync();

            await page.WaitForNavigationFromAsync(loginPageUrl);

            await navigator.WaitUntilLoggedIn(page, cancellationToken);

            return new Result<WebPage>(page.Url != loginPageUrl, new WebPage(request.Site,page));
        }
    }
}
