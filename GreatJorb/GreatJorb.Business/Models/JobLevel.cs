namespace GreatJorb.Business.Models;

[Flags]
public enum JobLevel
{
    Unknown = 0,

    [Display(Name="Entry Level")]
    EntryLevel = 1,

    [Display(Name = "Mid Level")]
    MidLevel = 2,

    [Display(Name = "Mid-Senior Level")]
    MidSeniorLevel = 4,

    [Display(Name = "Senior Level")]
    SeniorLevel = 8
}
