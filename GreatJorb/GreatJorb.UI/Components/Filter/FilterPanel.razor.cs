namespace GreatJorb.UI.Components.Filter;

public partial class FilterPanel
{
    public decimal? SalaryK
    {
        get
        {
            if (Filter.Salary == null)
                return null;
            return Filter.Salary.Value / 1000.0m;
        }
        set
        {
            if (value == null || value == 0)
                Filter.Salary = null;
            else
                Filter.Salary = value.Value * 1000.0m;
        }
    }

    [Parameter]
    public EventCallback<JobFilter> SearchRequested { get; set; }

    [Parameter]
    public JobFilter Filter { get; set; }

    public async Task SearchButtonClicked()
    {
        await SearchRequested.InvokeAsync(Filter);
    }
}
