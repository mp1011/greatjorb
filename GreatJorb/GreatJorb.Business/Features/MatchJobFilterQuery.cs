namespace GreatJorb.Business.Features;

public record MatchJobFilterQuery(JobPosting Job, bool HasKeywordMatches, JobFilter Filter) : IRequest<FilterMatch[]>
{
    public class Handler : IRequestHandler<MatchJobFilterQuery, FilterMatch[]>
    {
        public Task<FilterMatch[]> Handle(MatchJobFilterQuery request, CancellationToken cancellationToken)
        {
            List<FilterMatch> matches = new();

            if (request.Filter.JobLevelFilter != JobLevel.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.JobLevelFilter),
                    filter: request.Filter.JobLevelFilter,
                    actual: request.Job.JobLevel,
                    isUnknown: p => p == JobLevel.Unknown));


            if (request.Filter.WorkplaceTypeFilter != WorkplaceType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.WorkplaceTypeFilter),
                    filter: request.Filter.WorkplaceTypeFilter,
                    actual: request.Job.WorkplaceType,
                    isUnknown: p => p == WorkplaceType.Unknown));

            if (request.Filter.SalaryTypeFilter != SalaryType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.SalaryTypeFilter),
                    filter: request.Filter.SalaryTypeFilter,
                    actual: request.Job.SalaryType,
                    isUnknown: p => p == SalaryType.Unknown));

            if (request.Filter.JobTypeFilter != JobType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.JobTypeFilter),
                    filter: request.Filter.JobTypeFilter,
                    actual: request.Job.JobType,
                    isUnknown: p => p == JobType.Unknown));

            if(request.Filter.Salary.HasValue)
            {
                if (request.Job.SalaryMax == null)
                    matches.Add(new FilterMatch(FilterMatchLevel.Unknown, nameof(JobFilter.Salary)));
                else if(request.Job.SalaryMax.GetValueOrDefault() < request.Filter.Salary)
                    matches.Add(new FilterMatch(FilterMatchLevel.NegativeMatch, nameof(JobFilter.Salary)));
                else
                    matches.Add(new FilterMatch(FilterMatchLevel.PositiveMatch, nameof(JobFilter.Salary)));
            }

            if (!request.HasKeywordMatches)
                matches.Add(new FilterMatch(FilterMatchLevel.NegativeMatch, nameof(JobFilter.Query)));
            else
                matches.Add(new FilterMatch(FilterMatchLevel.PositiveMatch, nameof(JobFilter.Query)));

            return Task.FromResult(matches.ToArray());
        }

        private FilterMatch CheckEnum<T>(string field, T filter, T actual, Func<T, bool> isUnknown) where T : struct, Enum
        {
            if (isUnknown(actual))
                return new FilterMatch(FilterMatchLevel.Unknown, field);
            else if (filter.HasFlag(actual))
                return new FilterMatch(FilterMatchLevel.PositiveMatch, field);
            else
                return new FilterMatch(FilterMatchLevel.NegativeMatch, field);

        }
    }
}
