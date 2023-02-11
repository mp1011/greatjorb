namespace GreatJorb.Business.Models;

public enum Site
{
    None=0,
    LinkedIn = 1,

    [Display(Name = "Google Jobs")]
    GoogleJobs = 2,

    Indeed = 4,


    [Display(Name = "Simply Hired")]
    SimplyHired = 8,

    Dice = 16,

    Monster = 32,
}
