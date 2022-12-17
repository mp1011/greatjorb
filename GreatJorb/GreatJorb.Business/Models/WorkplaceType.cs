namespace GreatJorb.Business.Models;

[Flags]
public enum WorkplaceType
{
    Unknown = 0,
    OnSite = 1,
    Remote = 2,
    Hybrid = 4
}
