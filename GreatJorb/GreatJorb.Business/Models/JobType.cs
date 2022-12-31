namespace GreatJorb.Business.Models;

[Flags]
public enum JobType
{
    Unknown = 0,
    Contract = 1,

    [Display(Name = "Part-Time")]
    PartTime = 2,

    [Display(Name = "Full-Time")]
    FullTime = 4
}
