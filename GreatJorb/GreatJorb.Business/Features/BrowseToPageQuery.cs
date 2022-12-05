namespace GreatJorb.Business.Features;

public record class BrowseToPageQuery(string Url) : IRequest<IPage>
{
    public class Handler : IRequestHandler<BrowseToPageQuery,IPage>
    {
        private readonly BrowserProvider _browserProvider;

        public Handler(BrowserProvider browserProvider)
        {
            _browserProvider = browserProvider;
        }

        public async Task<IPage> Handle(BrowseToPageQuery request, CancellationToken cancellationToken)
        {
            IBrowser browser = await _browserProvider
                .GetBrowser();

            IPage page = await browser.NewPageAsync();
            await page.GoToAsync(request.Url);

            return page;
        }
    }
}
