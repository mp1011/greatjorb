namespace GreatJorb.Business.Models;

[Flags]
public enum WorkplaceType
{
    Unknown = 0,

    [Display(Name = "On-Site")]
    OnSite = 1,

    Remote = 2,
    Hybrid = 4
}
