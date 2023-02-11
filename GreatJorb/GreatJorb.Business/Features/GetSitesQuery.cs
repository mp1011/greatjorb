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

            if (sites == null || sites.Value.HasFlag(Site.Dice))
            {
                yield return new WebSite("Dice", "https://www.dice.com/", RequiresCredentials: true);
            }

            if (sites == null || sites.Value.HasFlag(Site.Monster))
            {
                yield return new WebSite("Monster", "https://www.monster.com/", RequiresCredentials: true);
            }

            if (sites == null || sites.Value.HasFlag(Site.Indeed))
            {
                yield return new WebSite("Indeed", "https://www.indeed.com/", RequiresCredentials: true);
            }

            if (sites == null || sites.Value.HasFlag(Site.SimplyHired))
            {
                yield return new WebSite("Simply Hired", "https://www.simplyhired.com/", RequiresCredentials: false);
            }
        }
    }
}
