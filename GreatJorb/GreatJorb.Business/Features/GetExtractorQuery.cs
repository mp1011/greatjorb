namespace GreatJorb.Business.Features;

public record GetExtractorQuery(WebSite Site) : IRequest<IJobPostingExtractor?>
{
    public class Handler : IRequestHandler<GetExtractorQuery, IJobPostingExtractor?>
    {
        private readonly IJobPostingExtractor[] _extractors;

        public Handler(IEnumerable<IJobPostingExtractor> webSiteNavigators)
        {
            _extractors = webSiteNavigators.ToArray();
        }

        public Task<IJobPostingExtractor?> Handle(GetExtractorQuery request, CancellationToken cancellationToken)
        {
            var extractor = _extractors.FirstOrDefault(p => p.WebsiteName == request.Site.Name);
            return Task.FromResult(extractor);
        }
    }
}
