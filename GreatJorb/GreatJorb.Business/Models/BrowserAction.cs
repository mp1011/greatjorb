namespace GreatJorb.Business.Models;

public enum BrowserAction
{
    Unknown=0,
    Navigate=1,
    FailedNavigationRetrying=2,
    FatalError=3,
    ManualCaptcha=4
}
