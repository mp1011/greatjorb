namespace GreatJorb.Business.Models;

public record JobPostingSearchResult(JobPosting[] MatchesFilter, JobPosting[] DoesNotMatchFilter)
{
    public static JobPostingSearchResult Empty { get; } = new JobPostingSearchResult(Array.Empty<JobPosting>(), Array.Empty<JobPosting>());
}

