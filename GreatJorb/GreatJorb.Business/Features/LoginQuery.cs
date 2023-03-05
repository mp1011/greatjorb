namespace GreatJorb.Business.Features;

public record LoginQuery(WebSite Site) : IRequest<Result<WebPage>>
{

    class Handler : IRequestHandler<LoginQuery, Result<WebPage>>
    {
        private readonly IMediator _mediator;
        private readonly ISecureSettingsService _settingsService;

        public Handler(IMediator mediator, ISecureSettingsService settingsService)
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

            if (!await navigator.IsLoginRequired(page, cancellationToken))
            {
                return new Result<WebPage>(true, new WebPage(request.Site, page));
            }

            string? userName = await _settingsService.GetSiteUserName(request.Site);
            string? password = await _settingsService.GetSitePassword(request.Site);

            if (userName == null || password == null)
                throw new Exception($"Credentials for {request.Site} not found");

            await navigator
                .GetLoginElement(page, cancellationToken)
                .SetText(page, userName)
                .ThrowNavigationErrorIfNull(page,_mediator,"Login Element", cancellationToken);

            await navigator
                .GetPasswordElement(page, cancellationToken)
                .SetText(page, password)
                .ThrowNavigationErrorIfNull(page, _mediator, "Password Element", cancellationToken);

            await navigator
                .GetLoginButton(page, cancellationToken)
                .ThrowNavigationErrorIfNull(page, _mediator, "Login Button", cancellationToken)
                .ClickAsync();

            await page.WaitForNavigationFromAsync(loginPageUrl);

            _mediator.Publish(new BrowserPageChanged(page, "Waiting to be logged in"));

            await navigator.WaitUntilLoggedIn(page, cancellationToken);

            return new Result<WebPage>(true, new WebPage(request.Site,page));
        }
    }
}
