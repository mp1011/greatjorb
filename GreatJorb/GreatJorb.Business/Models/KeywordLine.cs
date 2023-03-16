namespace GreatJorb.Business.Models;

public enum KeywordLineType
{
    Query,
    PositiveKeyword,
    NeutralKeyword,
    NegativeKeyword
}

public record KeywordLine(string Line, KeywordLineType Type)
{
}
