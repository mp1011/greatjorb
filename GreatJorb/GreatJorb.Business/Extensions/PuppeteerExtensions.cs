namespace GreatJorb.Business.Extensions;

public static class PuppeteerExtensions
{

    public static async Task WaitForNavigationFromAsync(this IPage page, string originalUrl)
    {
        if (page.Url != originalUrl)
            return;

        await page.WaitForNavigationAsync();
    }

    public static async Task SetText(this IElementHandle inputElement, 
        IPage page, 
        string text, 
        bool clearExistingText=true,
        bool pressEnter = false)
    {
        await inputElement.FocusAsync();

        if (clearExistingText)
        {
            await page.Keyboard.DownAsync(Key.Shift);
            await page.Keyboard.PressAsync(Key.Home);
            await page.Keyboard.UpAsync(Key.Shift);
            await page.Keyboard.PressAsync(Key.Delete);
        }

        await page.Keyboard.TypeAsync(text);

        if(pressEnter)
            await page.Keyboard.PressAsync(Key.Enter);
    }

    public static async Task<string> GetTextAsync(this IElementHandle element, string querySelector)
    {
        var textElement = await element.QuerySelectorAsync(querySelector);
        if (textElement == null)
            return string.Empty;

        var text = await textElement.GetInnerHTML();
        return text
            .Replace("\n","")
            .Trim();
    }

    public static async Task<int?> TryGetNumberAsync(this IElementHandle element)
    {
        var text = await element.GetInnerText();
        return text.TryParseIntOrDefault();
    }


    public static async Task<string> GetInnerHTML(this Task<IElementHandle> elementTask)
    {
        var element = await elementTask;
        if (element == null)
            return String.Empty;

        return await element.GetInnerHTML();
    }

    public static async Task<string> GetInnerHTML(this IElementHandle element)
    {
        if (element == null)
            return string.Empty;

        try
        {
            return await element.EvaluateFunctionAsync<string>("e=>e.innerHTML", element);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static async Task<IElementHandle?> SetText(
        this Task<IElementHandle?> inputElementTask, 
        IPage page, 
        string text, 
        bool clearExistingText=true,
        bool pressEnter=false)
    {
        var inputElement = await inputElementTask;
        if (inputElement != null)
        {
            await inputElement.SetText(page, text, clearExistingText, pressEnter);
        }

        return inputElement;
    }

    public static async Task<IElementHandle?> GetElementLabelledBy(this IPage page, string label)
    {
        var ariaLabelled = await page
            .QuerySelectorAllAsync($"[aria-label='{label}']:not([aria-hidden='true'])")
            .VisibleOnly();

        if (ariaLabelled != null && ariaLabelled.Length == 1)
            return ariaLabelled[0];

        var labelHandle = await page.GetLabel(label);
        if (labelHandle == null)
            return null;

        var labelFor = await labelHandle.GetAttribute("for");

        var inputElement = await page.QuerySelectorAsync($"#{labelFor}");

        return inputElement;
    }

    public static async Task<IElementHandle[]> VisibleOnly(this Task<IElementHandle[]> elementsTask)
    {
        List<IElementHandle> visibleElements = new();

        var elements = await elementsTask;
        foreach (var element in elements)
        {
            var visible = await element.CheckVisible();
            if (visible)
                visibleElements.Add(element);
        }
        return visibleElements.ToArray();
    }

    public static async Task<bool> CheckVisible(this IElementHandle element)
    {
        var boundingBox = await element.BoundingBoxAsync();
        return boundingBox != null && boundingBox.Height > 0;
    }

    public static async Task ClickAsync(this Task<IElementHandle?> elementTask)
    {
        var element = await elementTask;
        if (element == null)
            return;

        await element.ClickAsync();
    }

    public static async Task JsClickAsync(this Task<IElementHandle?> elementTask)
    {
        var element = await elementTask;
        await element.JsClickAsync();
    }
    public static async Task JsClickAsync(this IElementHandle? element)
    {
        if (element == null)
            return;

        try
        {
            await element.EvaluateFunctionAsync<string>("e=>e.click()", element);
        }
        catch
        {
        }
    }

    public static async Task<IElementHandle?> GetLabel(this IPage page, string label)
    {
        var labelHandles = await page.QuerySelectorAllAsync("label");

        foreach (var labelHandle in labelHandles)
        {
            string content = await labelHandle.GetInnerHTML(page);
            if (content.Trim().Equals(label))
                return labelHandle;
        }

        return null;
    }

    public static async Task<string> GetInnerTextAsync(this IPage page, string selector)
    {
        var element = await page.QuerySelectorAsync(selector);
        return await element.GetInnerText();
    }

    public static async Task<string[]> GetInnerTextAsync(this Task<IElementHandle[]> elementsTask)
    {
        var elements = await elementsTask;
        return await elements.GetInnerTextAsync();
    }

    public static async Task<string[]> GetInnerTextAsync(this IElementHandle[] elements)
    {
        List<string> text = new();
        foreach(var element in elements)
        {
            text.Add(await element.GetInnerText());
        }
        return text.ToArray();
    }

    private static async Task<string> GetInnerText(this IElementHandle? element)
    {
        if (element == null)
            return string.Empty;

        try
        {
            var text = await element.EvaluateFunctionAsync<string>("e=>e.innerText", element);
            return text.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    public static async Task<string> GetInnerHTML(this IElementHandle? element, IPage page)
    {
        if (element == null)
            return string.Empty;

        try
        {
            return await page.EvaluateFunctionAsync<string>("e=>e.innerHTML", element);
        }
        catch
        {
            return string.Empty;
        }
    }
    public static async Task<string> GetAttribute(this Task<IElementHandle?> elementTask, string attribute)
    {
        var element = await elementTask;
        if (element == null)
            return string.Empty;

        return await element.GetAttribute(attribute);

    }

    public static async Task<string> GetAttribute(this IElementHandle? element, string attribute)
    {
        if (element == null)
            return string.Empty;

        try
        {
            var result = await element.EvaluateFunctionAsync($"e => e.getAttribute('{attribute}')");
            return result.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

}
