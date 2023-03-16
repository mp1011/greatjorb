namespace GreatJorb.Business.Models;

public class WatchWord
{
    public string Phrase { get; set; } = "";

    public bool? IsGood { get; set; }

    public WatchWord(string phrase, bool? isGood)
    {
        Phrase = phrase;
        IsGood = isGood;
    }

    public WatchWord()
    {
    }
}
