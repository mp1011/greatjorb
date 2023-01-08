namespace GreatJorb.Business.Features;

public record GetSitesQuery(Site? Sites=null) : IRequest<WebSite[]>
{
    public class Handler : IRequestHandler<GetSitesQuery, WebSite[]>
    {
        public Task<WebSite[]> Handle(GetSitesQuery request, CancellationToken cancellationToken)
        {
            var result = GetSites(request.Sites).ToArray();
            return Task.FromResult(result);
        }

        private IEnumerable<WebSite> GetSites(Site? sites)
        {
            if (sites == null || sites.Value.HasFlag(Site.LinkedIn))
            {
                yield return new WebSite("LinkedIn", "https://www.linkedin.com/", RequiresCredentials: true);
            }

            if (sites == null || sites.Value.HasFlag(Site.GoogleJobs))
            {
                yield return new WebSite("Google Jobs", "https://www.google.com/", RequiresCredentials: false);
            }
        }
    }
}
