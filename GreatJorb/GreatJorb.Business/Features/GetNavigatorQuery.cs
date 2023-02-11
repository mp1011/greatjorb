namespace GreatJorb.Business.Features;

public record GetNavigatorQuery(WebSite Site) : IRequest<IWebSiteNavigator?>
{
    public class Handler : IRequestHandler<GetNavigatorQuery, IWebSiteNavigator?>
    {
        private readonly IWebSiteNavigator[] _webSiteNavigators;

        public Handler(IEnumerable<IWebSiteNavigator> webSiteNavigators)
        {
            _webSiteNavigators = webSiteNavigators.ToArray();
        }

        public Task<IWebSiteNavigator?> Handle(GetNavigatorQuery request, CancellationToken cancellationToken)
        {
            var navigator = _webSiteNavigators.FirstOrDefault(p => p.Website.GetDisplayName() == request.Site.Name);
            return Task.FromResult(navigator);
        }
    }
}
