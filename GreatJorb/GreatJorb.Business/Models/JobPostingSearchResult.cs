namespace GreatJorb.Business.Models;

public record JobPostingSearchResult(JobPosting Job, WebSite Site, FilterMatch[] FilterMatches, KeywordLine[] KeywordLines) : IComparable<JobPostingSearchResult>
{
    public int CompareTo(JobPostingSearchResult? other)
    {
        if (other == null)
            return int.MinValue;
        else
            return other.MatchScore - MatchScore;
    }

    public int MatchScore => FilterMatches.Sum(p => p.Score);
}

