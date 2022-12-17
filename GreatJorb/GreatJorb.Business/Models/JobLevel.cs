namespace GreatJorb.Business.Models
{
    [Flags]
    public enum JobLevel
    {
        Unknown = 0,
        EntryLevel = 1,
        MidLevel = 2,
        MidSeniorLevel = 4,
        SeniorLevel = 8
    }
}
