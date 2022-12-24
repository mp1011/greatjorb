namespace GreatJorb.UI.Components.Shared;

public partial class ColorCodedBadge
{
    [Parameter]
    public bool Neutral { get; set; }

    [Parameter]
    public bool? IsMatch { get; set; }

    [Parameter]
    public string Text { get; set; } = "";

    public string BadgeCss
    {
        get
        {
            if (Neutral)
                return "bg-info";
            else
                return IsMatch switch
                {
                    null => "bg-warning text-dark",
                    true => "bg-success",
                    false => "bg-danger"
                };
        }
    }
}
