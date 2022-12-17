namespace GreatJorb.Business.Models;

[Flags]
public enum JobType
{
    Unknown = 0,
    Contract = 1,
    PartTime = 2,
    FullTime = 4
}
