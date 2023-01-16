namespace GreatJorb.Business.Extensions;

public static class PuppeteerExtensions
{
    private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);


    public static async Task<string[]> ExtractTextFromLeafNodes(this IElementHandle element, string selector)
    {
        var elements = await element
            .QuerySelectorAllAsync(selector)
            .VisibleOnly();

        List<string> lines = new();

        foreach(var childElement in elements)
        {
            var innerHtml = await childElement.GetInnerHTML();
            if (innerHtml.Contains("<") && innerHtml.Contains(">"))
                continue;

            var innerText = await childElement.GetInnerText();
            if (innerText.IsNullOrEmpty())
                continue;

            lines.Add(innerText);
        }

        return lines.ToArray();
    }

    public static async Task<IElementHandle?> GetElementByInnerText(this IPage page, 
        string selector, 
        string innerText, 
        CancellationToken cancellationToken, 
        bool wildCardMatch=false)
    {
        DateTime begin = DateTime.Now;

        while (begin.TimeSince() <= DefaultTimeout)
        {
            var elements = await page
                .QuerySelectorAllAsync(selector)
                .VisibleOnly();

            foreach (var item in elements)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string elementText = await item.GetInnerText();
                elementText = elementText.Trim();

                if (wildCardMatch && elementText.IsWildcardMatch(innerText))
                    return item;
                else if (elementText.Equals(innerText, StringComparison.OrdinalIgnoreCase))
                    return item;
            }
        }

        return null;
    }

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

    public static async Task<string> GetInnerHTML(this Task<IElementHandle?> elementTask)
    {
        if (elementTask == null)
            return String.Empty;

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

    public static async Task<IElementHandle?> GetElementLabelledBy(this IPage page, string label, CancellationToken cancellationToken)
    {
        while(true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ariaLabelled = await page
                .QuerySelectorAllAsync($"[aria-label='{label}']:not([aria-hidden='true'])")
                .VisibleOnly();

            if (ariaLabelled != null && ariaLabelled.Length == 1)
                return ariaLabelled[0];

            var labelHandle = await page.GetLabel(label);
            if (labelHandle == null)
                continue;

            var labelFor = await labelHandle.GetAttribute("for");

            var inputElement = await page.QuerySelectorAllAsync($"#{labelFor}")
                .FirstVisibleOrDefault();

            if(inputElement != null)
                return inputElement;
        }

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

    public static async Task<IElementHandle?> FirstVisibleOrDefault(this Task<IElementHandle[]> elementsTask)
    {
        var elements = await elementsTask;
        foreach (var element in elements)
        {
            var visible = await element.CheckVisible();
            if (visible)
                return element;
        }

        return null;
    }

    public static async Task<bool> CheckVisible(this IElementHandle element)
    {
        var ariaHidden = await element.GetAttribute("aria-hidden");
        if (ariaHidden != null && ariaHidden.Equals("true", StringComparison.OrdinalIgnoreCase))
            return false;

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
            string innerText = await labelHandle.GetInnerText();

            foreach(var line in innerText.Split('\n').Where(p=>p.Length != 0))
            {
                if (line.Trim().Equals(label, StringComparison.OrdinalIgnoreCase))
                    return labelHandle;
            }
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

    public static async Task<string> GetInnerText(this IElementHandle? element)
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


    public static async Task<Size> GetViewportSize(this IPage page)
    {
        var viewportWidth = await page.EvaluateFunctionAsync<int>("e => window.innerWidth");
        var viewportHeight = await page.EvaluateFunctionAsync<int>("e => window.innerHeight");

        return new(viewportWidth, viewportHeight);
    }

    public static async Task<IElementHandle?> GetElementByPoint(this IPage page, double pctX, double pctY)
    {
        var viewportSize = await page.GetViewportSize();
        var pixelX = viewportSize.Width * pctX;
        var pixelY = viewportSize.Height * pctY;

        var elementAtPoint = await page.EvaluateFunctionReturnElementAsync($"document.elementFromPoint({pixelX},{pixelY})");
        return elementAtPoint;
    }

    /// <summary>
    /// Returns the first ancestor that matches the given condition
    /// </summary>
    /// <param name="element"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static async Task<IElementHandle?> GetAncestor(this IElementHandle? element, 
        IPage page,
        Func<IElementHandle, Task<bool>> condition, 
        CancellationToken cancellationToken)
    {

        while (element != null)
        {
            if (await condition(element))
                return element;

            element = await element.ParentElementAsync(page, cancellationToken);
        }

        return null;
    }


    /// <summary>
    /// Returns the first ancestor that matches the given condition
    /// </summary>
    /// <param name="element"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static async Task<IElementHandle?> GetAncestor(this Task<IElementHandle?> elementTask,
        IPage page,
        Func<IElementHandle, Task<bool>> condition,
        CancellationToken cancellationToken)
    {
        var element = await elementTask;
        if (element == null)
            return null;
        else 
            return await element.GetAncestor(page, condition, cancellationToken);
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
    public static async Task<bool?> GetBooleanAttribute(this IElementHandle? element, string attribute)
    {
        var attributeValue = await element.GetAttribute(attribute);
        if (attributeValue.IsNullOrEmpty())
            return null;

        return attributeValue.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public static async Task<string> GetAttribute(this IElementHandle? element, string attribute)
    {
        if (element == null)
            return string.Empty;

        try
        {
            var result = await element.EvaluateFunctionAsync($"e => e.getAttribute('{attribute}')");
            if (result == null)
                return String.Empty;

            return result.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="page"></param>
    /// <param name="jsFunction">Example: e.myFunction()</param>
    /// <returns></returns>
    public static async Task<IElementHandle?> EvaluateFunctionReturnElementAsync(this IElementHandle element, IPage page, string jsFunction)
    {
        var elementId = await element.EvaluateFunctionAsync<string>(@"e => { 
                var element = " + jsFunction + @";
                if(!element)
                    return '';

                if(!element.id)
                {
                    element.id = '" + "dummy" + Guid.NewGuid().ToString() + @"';
                }

                return element.id;
            }");

        if (elementId.IsNullOrEmpty())
            return null;

        return await page.QuerySelectorAsync("#" + elementId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="jsFunction">Example: "document.getElementById('123')</param>
    /// <returns></returns>
    public static async Task<IElementHandle?> EvaluateFunctionReturnElementAsync(this IPage page, string jsFunction)
    {
        var elementId = await page.EvaluateFunctionAsync<string>(@"e => { 
                var element = " + jsFunction + @";

                if(!element)
                {
                    return '';
                }

                if(!element.id)
                {
                    element.id = '" + "dummy" + Guid.NewGuid().ToString() + @"';
                }

                return element.id;
            }");

        if (elementId.IsNullOrEmpty())
            return null;

        return await page.QuerySelectorAsync("#" + elementId);
    }

    public static async Task<IElementHandle?> NextElementAsync(this IElementHandle element, IPage page, CancellationToken cancellationToken)
    {

        var nextSibling = await element.EvaluateFunctionReturnElementAsync(page, "e.nextElementSibling"); 

        if(nextSibling == null)
        {
            var parent = await element.EvaluateFunctionReturnElementAsync(page,"e.parentElement");
            if (parent == null)
                return null;

            return await parent.NextElementAsync(page, cancellationToken);
        }

        return nextSibling;
    }

    public static async Task<IElementHandle?> ParentElementAsync(this IElementHandle element, IPage page, CancellationToken cancellationToken)
    {
        return await element.EvaluateFunctionReturnElementAsync(page, "e.parentElement");
    }

    public static async Task<IElementHandle?> ParentElementAsync(this Task<IElementHandle?> elementTask, IPage page, CancellationToken cancellationToken)
    {
        var element = await elementTask;
        if (element == null)
            return null;

        return await element.EvaluateFunctionReturnElementAsync(page, "e.parentElement");
    }

    public static async Task WaitForDOMIdle(this IPage page, CancellationToken cancellationToken)
    {
        DateTime start = DateTime.Now;

        var body = await page.QuerySelectorAsync("body");
        var html = await body.GetInnerHTML();

        while(!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
            var newHtml = await body.GetInnerHTML();
            if(newHtml.Equals(html))
            {
                return;
            }

            html = newHtml;
        }
    }
}
