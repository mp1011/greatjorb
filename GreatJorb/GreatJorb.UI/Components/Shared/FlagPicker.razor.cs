namespace GreatJorb.UI.Components.Shared;

public partial class FlagPicker<T>
    where T: struct, Enum
{
    public T[] Options { get; set; } = Array.Empty<T>();

    public T CurrentSelection { get; set; } = default;

    [Parameter]
    public EventCallback<T> SelectionChanged { get; set; }

    private int CurrentSelectionInt => (int)(object)CurrentSelection;

    protected override void OnInitialized()
    {
        Options = Enum.GetValues<T>()
            .Where(p => (int)(object)p != 0)
            .ToArray();
    }

    public bool IsChecked(T option) => CurrentSelection.HasFlag(option);
    
    public async Task SetChecked(T flag, bool state)
    {
        int flagInt = (int)(object)flag;
        if(state)
        {
            CurrentSelection = (T)(object)(CurrentSelectionInt | flagInt);
        }
        else
        {
            CurrentSelection = (T)(object)(CurrentSelectionInt & ~flagInt);
        }

        await SelectionChanged.InvokeAsync(CurrentSelection);
    }

    public async Task ToggleChecked(T flag)
    {
        await SetChecked(flag, !IsChecked(flag));
    }
}
