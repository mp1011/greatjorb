namespace GreatJorb.Business.Features;

public record class BrowseToPageQuery(string Url) : IRequest<IPage>
{
    public class Handler : IRequestHandler<BrowseToPageQuery,IPage>
    {
        private readonly BrowserProvider _browserProvider;
        private readonly IMediator _mediator;
        private readonly ISettingsService _settingsService;

        public Handler(BrowserProvider browserProvider, IMediator mediator, ISettingsService settingsService)
        {
            _browserProvider = browserProvider;
            _mediator = mediator;
            _settingsService = settingsService;
        }

        public async Task<IPage> Handle(BrowseToPageQuery request, CancellationToken cancellationToken)
        {
            IBrowser browser = await _browserProvider
                .GetBrowser();

            IPage page = new BrowserAutomation(await browser.NewPageAsync(), _mediator, _settingsService);

            await page.GoToAsync(request.Url);

            return page;
        }
    }
}
