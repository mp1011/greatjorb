namespace GreatJorb.Business.Models;

public record JobPostingSearchResult(JobPosting Job, FilterMatch[] FilterMatches)
{
}

