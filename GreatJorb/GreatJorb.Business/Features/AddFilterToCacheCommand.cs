namespace GreatJorb.Business.Features;

public record AddFilterToCacheCommand(JobFilter Filter) : IRequest<JobFilterCacheResult>
{
    public class Handler : IRequestHandler<AddFilterToCacheCommand, JobFilterCacheResult>
    {
        private readonly LocalDataContextProvider _contextProvider;

        public Handler(LocalDataContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<JobFilterCacheResult> Handle(AddFilterToCacheCommand request, CancellationToken cancellationToken)
        {
            using var context = _contextProvider.GetContext();

            request.Filter.Id = Guid.NewGuid();

            var header = new JobFilterCacheHeader(request.Filter.Id, DateTime.Now);
            await context.Store(header);
            await context.Store(request.Filter);

            return new JobFilterCacheResult(header.Date, request.Filter);
        }
    }
}
