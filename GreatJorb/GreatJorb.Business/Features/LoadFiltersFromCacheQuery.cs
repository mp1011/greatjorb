namespace GreatJorb.Business.Features;

public record LoadFiltersFromCacheQuery() : IRequest<JobFilterCacheResult[]>
{
    public class Handler : IRequestHandler<LoadFiltersFromCacheQuery, JobFilterCacheResult[]>
    {
        private readonly LocalDataContextProvider _contextProvider;

        public Handler(LocalDataContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<JobFilterCacheResult[]> Handle(LoadFiltersFromCacheQuery request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            var headers = await context.Retrieve<JobFilterCacheHeader>(p => true);         
            List<JobFilterCacheResult> results = new();

            foreach(var header in headers)
            {
                var filter = await context.Retrieve<JobFilter>(header.Id.ToString());
                if(filter != null)
                    results.Add(new JobFilterCacheResult(header.Date, filter));
            }

            return results.ToArray();
        }
    }
}
