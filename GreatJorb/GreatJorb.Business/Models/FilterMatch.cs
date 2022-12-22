namespace GreatJorb.Business.Models;

public enum FilterMatchLevel
{
    Unknown,
    PositiveMatch,
    NegativeMatch,     
}

public record FilterMatch(FilterMatchLevel Level, string Field, string Description);

