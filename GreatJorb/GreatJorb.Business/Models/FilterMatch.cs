namespace GreatJorb.Business.Models;

public enum FilterMatchLevel
{
    Unknown,
    PositiveMatch,
    NegativeMatch,     
}

public record FilterMatch(FilterMatchLevel Level, string Field)
{
    public int Score
    {
        get
        {
            int baseScore = 10;
            if (Field == nameof(JobFilter.Salary))
                baseScore = 20;

            return Level switch
            {
                FilterMatchLevel.PositiveMatch => baseScore,
                FilterMatchLevel.NegativeMatch => -baseScore,
                _ => 0
            };
        }
    }
}


