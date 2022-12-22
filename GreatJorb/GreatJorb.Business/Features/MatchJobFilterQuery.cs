namespace GreatJorb.Business.Features;

public record MatchJobFilterQuery(JobPosting Job, JobFilter Filter) : IRequest<FilterMatch[]>
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
                    isUnknown: p => p == JobLevel.Unknown,
                    unknownText: "Job Level was not provided",
                    noMatchText: "Job Level does not match filter",
                    matchText: "Job Level matches filter"));


            if (request.Filter.WorkplaceTypeFilter != WorkplaceType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.WorkplaceTypeFilter),
                    filter: request.Filter.WorkplaceTypeFilter,
                    actual: request.Job.WorkplaceType,
                    isUnknown: p => p == WorkplaceType.Unknown,
                    unknownText: "Workplace Type was not provided",
                    noMatchText: "Workplace Type does not match filter",
                    matchText: "Workplace Type matches filter"));

            if (request.Filter.SalaryTypeFilter != SalaryType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.SalaryTypeFilter),
                    filter: request.Filter.SalaryTypeFilter,
                    actual: request.Job.SalaryType,
                    isUnknown: p => p == SalaryType.Unknown,
                    unknownText: "Salary Type was not provided",
                    noMatchText: "Salary Type does not match filter",
                    matchText: "Salary Type matches filter"));

            if (request.Filter.JobTypeFilter != JobType.Unknown)
                matches.Add(CheckEnum(
                    field: nameof(JobFilter.JobTypeFilter),
                    filter: request.Filter.JobTypeFilter,
                    actual: request.Job.JobType,
                    isUnknown: p => p == JobType.Unknown,
                    unknownText: "Job Type was not provided",
                    noMatchText: "Job Type does not match filter",
                    matchText: "Job Type matches filter"));

            if(request.Filter.Salary.HasValue)
            {
                if (request.Job.SalaryMax == null)
                    matches.Add(new FilterMatch(FilterMatchLevel.Unknown, nameof(JobFilter.Salary), "Salary was not provided"));
                else if(request.Job.SalaryMax.GetValueOrDefault() < request.Filter.Salary)
                    matches.Add(new FilterMatch(FilterMatchLevel.NegativeMatch, nameof(JobFilter.Salary), "Salary does not match filter"));
                else
                    matches.Add(new FilterMatch(FilterMatchLevel.PositiveMatch, nameof(JobFilter.Salary), "Salary matches filter"));
            }

            if (!request.Job.KeywordLines.Any())
                matches.Add(new FilterMatch(FilterMatchLevel.NegativeMatch, nameof(JobFilter.Query), "Keyword was not found in description"));
            else
                matches.Add(new FilterMatch(FilterMatchLevel.PositiveMatch, nameof(JobFilter.Query), "Keyword was found in description"));

            return Task.FromResult(matches.ToArray());
        }

        private FilterMatch CheckEnum<T>(string field, T filter, T actual, Func<T, bool> isUnknown,
            string unknownText,
            string noMatchText,
            string matchText) where T : struct, Enum
        {
            if (isUnknown(actual))
                return new FilterMatch(FilterMatchLevel.Unknown, field, unknownText);
            else if (filter.HasFlag(actual))
                return new FilterMatch(FilterMatchLevel.PositiveMatch, field, matchText);
            else
                return new FilterMatch(FilterMatchLevel.NegativeMatch, field, noMatchText);

        }
    }
}
