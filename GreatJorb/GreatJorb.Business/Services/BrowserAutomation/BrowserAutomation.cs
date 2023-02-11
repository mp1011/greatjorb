namespace GreatJorb.Business.Services.BrowserAutomation;

public class BrowserAutomation : IPage
{
    private readonly IMediator _mediator;
    private readonly ISettingsService _settings;
    private readonly IPage _page;


    public DateTime LastRequestTime { get; set; }

    public BrowserAutomation(IPage page, IMediator mediator, ISettingsService settingsService)
    {
        _mediator = mediator;
        _settings = settingsService;
        _page = page;
    }

    public async Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[]? waitUntil = null)
    {
        int tries = _settings.MaxNavigationRetries;

        while(tries > 0)
        {
            try
            {
                return await _page.GoToAsync(url, timeout, waitUntil);
            }
            catch
            {
                await Task.Delay(100);
                tries--;
            }
        }

        throw new Exception("Failed to navigate");
    }

    public async Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        int tries = _settings.MaxNavigationRetries;

        while (tries > 0)
        {
            try
            {
                return await _page.GoToAsync(url, options);
            }
            catch
            {
                await Task.Delay(100);
                tries--;
            }
        }


        throw new Exception("Failed to navigate");
    }

    public async Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
    {
        int tries = _settings.MaxNavigationRetries;

        while (tries > 0)
        {
            try
            {
                return await _page.GoToAsync(url, waitUntil);
            }
            catch
            {
                await Task.Delay(100);
                tries--;
            }
        }

        throw new Exception("Failed to navigate");
    }

    #region IPage implementaion
    public IAccessibility Accessibility => _page.Accessibility;

    public IBrowser Browser => _page.Browser;

    public IBrowserContext BrowserContext => _page.BrowserContext;

    public ICDPSession Client => _page.Client;

    public ICoverage Coverage => _page.Coverage;

    public int DefaultNavigationTimeout { get => _page.DefaultNavigationTimeout; set => _page.DefaultNavigationTimeout = value; }
    public int DefaultTimeout { get => _page.DefaultTimeout; set => _page.DefaultTimeout = value; }

    public IFrame[] Frames => _page.Frames;

    public bool IsClosed => _page.IsClosed;

    public bool IsDragInterceptionEnabled => _page.IsDragInterceptionEnabled;

    public IKeyboard Keyboard => _page.Keyboard;

    public IFrame MainFrame => _page.MainFrame;

    public IMouse Mouse => _page.Mouse;

    public ITarget Target => _page.Target;

    public ITouchscreen Touchscreen => _page.Touchscreen;

    public ITracing Tracing => _page.Tracing;

    public string Url => _page.Url;

    public ViewPortOptions Viewport => _page.Viewport;

    public Worker[] Workers => _page.Workers;

    public event EventHandler Close
    {
        add
        {
            _page.Close += value;
        }

        remove
        {
            _page.Close -= value;
        }
    }

    public event EventHandler<ConsoleEventArgs> Console
    {
        add
        {
            _page.Console += value;
        }

        remove
        {
            _page.Console -= value;
        }
    }

    public event EventHandler<DialogEventArgs> Dialog
    {
        add
        {
            _page.Dialog += value;
        }

        remove
        {
            _page.Dialog -= value;
        }
    }

    public event EventHandler DOMContentLoaded
    {
        add
        {
            _page.DOMContentLoaded += value;
        }

        remove
        {
            _page.DOMContentLoaded -= value;
        }
    }

    public event EventHandler<PuppeteerSharp.ErrorEventArgs> Error
    {
        add
        {
            _page.Error += value;
        }

        remove
        {
            _page.Error -= value;
        }
    }

    public event EventHandler<FrameEventArgs> FrameAttached
    {
        add
        {
            _page.FrameAttached += value;
        }

        remove
        {
            _page.FrameAttached -= value;
        }
    }

    public event EventHandler<FrameEventArgs> FrameDetached
    {
        add
        {
            _page.FrameDetached += value;
        }

        remove
        {
            _page.FrameDetached -= value;
        }
    }

    public event EventHandler<FrameEventArgs> FrameNavigated
    {
        add
        {
            _page.FrameNavigated += value;
        }

        remove
        {
            _page.FrameNavigated -= value;
        }
    }

    public event EventHandler Load
    {
        add
        {
            _page.Load += value;
        }

        remove
        {
            _page.Load -= value;
        }
    }

    public event EventHandler<MetricEventArgs> Metrics
    {
        add
        {
            _page.Metrics += value;
        }

        remove
        {
            _page.Metrics -= value;
        }
    }

    public event EventHandler<PageErrorEventArgs> PageError
    {
        add
        {
            _page.PageError += value;
        }

        remove
        {
            _page.PageError -= value;
        }
    }

    public event EventHandler<PopupEventArgs> Popup
    {
        add
        {
            _page.Popup += value;
        }

        remove
        {
            _page.Popup -= value;
        }
    }

    public event EventHandler<RequestEventArgs> Request
    {
        add
        {
            _page.Request += value;
        }

        remove
        {
            _page.Request -= value;
        }
    }

    public event EventHandler<RequestEventArgs> RequestFailed
    {
        add
        {
            _page.RequestFailed += value;
        }

        remove
        {
            _page.RequestFailed -= value;
        }
    }

    public event EventHandler<RequestEventArgs> RequestFinished
    {
        add
        {
            _page.RequestFinished += value;
        }

        remove
        {
            _page.RequestFinished -= value;
        }
    }

    public event EventHandler<RequestEventArgs> RequestServedFromCache
    {
        add
        {
            _page.RequestServedFromCache += value;
        }

        remove
        {
            _page.RequestServedFromCache -= value;
        }
    }

    public event EventHandler<ResponseCreatedEventArgs> Response
    {
        add
        {
            _page.Response += value;
        }

        remove
        {
            _page.Response -= value;
        }
    }

    public event EventHandler<WorkerEventArgs> WorkerCreated
    {
        add
        {
            _page.WorkerCreated += value;
        }

        remove
        {
            _page.WorkerCreated -= value;
        }
    }

    public event EventHandler<WorkerEventArgs> WorkerDestroyed
    {
        add
        {
            _page.WorkerDestroyed += value;
        }

        remove
        {
            _page.WorkerDestroyed -= value;
        }
    }

    public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
    {
        return _page.AddScriptTagAsync(options);
    }

    public Task<IElementHandle> AddScriptTagAsync(string url)
    {
        return _page.AddScriptTagAsync(url);
    }

    public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
    {
        return _page.AddStyleTagAsync(options);
    }

    public Task<IElementHandle> AddStyleTagAsync(string url)
    {
        return _page.AddStyleTagAsync(url);
    }

    public Task AuthenticateAsync(Credentials credentials)
    {
        return _page.AuthenticateAsync(credentials);
    }

    public Task BringToFrontAsync()
    {
        return _page.BringToFrontAsync();
    }

    public Task ClickAsync(string selector, ClickOptions? options = null)
    {
        return _page.ClickAsync(selector, options);
    }

    public Task CloseAsync(PageCloseOptions? options = null)
    {
        return _page.CloseAsync(options);
    }

    public Task DeleteCookieAsync(params CookieParam[] cookies)
    {
        return _page.DeleteCookieAsync(cookies);
    }

    public void Dispose()
    {
        _page.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _page.DisposeAsync();
    }

    public Task EmulateAsync(DeviceDescriptor options)
    {
        return _page.EmulateAsync(options);
    }

    public Task EmulateCPUThrottlingAsync(decimal? factor = null)
    {
        return _page.EmulateCPUThrottlingAsync(factor);
    }

    public Task EmulateIdleStateAsync(EmulateIdleOverrides? idleOverrides = null)
    {
        return _page.EmulateIdleStateAsync(idleOverrides);
    }

    public Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
    {
        return _page.EmulateMediaFeaturesAsync(features);
    }

    public Task EmulateMediaTypeAsync(MediaType type)
    {
        return _page.EmulateMediaTypeAsync(type);
    }

    public Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
    {
        return _page.EmulateNetworkConditionsAsync(networkConditions);
    }

    public Task EmulateTimezoneAsync(string timezoneId)
    {
        return _page.EmulateTimezoneAsync(timezoneId);
    }

    public Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
    {
        return _page.EmulateVisionDeficiencyAsync(type);
    }

    public Task<JToken> EvaluateExpressionAsync(string script)
    {
        return _page.EvaluateExpressionAsync(script);
    }

    public Task<T> EvaluateExpressionAsync<T>(string script)
    {
        return _page.EvaluateExpressionAsync<T>(script);
    }

    public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
    {
        return _page.EvaluateExpressionHandleAsync(script);
    }

    public Task EvaluateExpressionOnNewDocumentAsync(string expression)
    {
        return _page.EvaluateExpressionOnNewDocumentAsync(expression);
    }

    public Task<JToken> EvaluateFunctionAsync(string script, params object[] args)
    {
        return _page.EvaluateFunctionAsync(script, args);
    }

    public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
    {
        return _page.EvaluateFunctionAsync<T>(script, args);
    }

    public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
    {
        return _page.EvaluateFunctionHandleAsync(pageFunction, args);
    }

    public Task EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
    {
        return _page.EvaluateFunctionOnNewDocumentAsync(pageFunction, args);
    }

    public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
    {
        return _page.ExposeFunctionAsync(name, puppeteerFunction);
    }

    public Task FocusAsync(string selector)
    {
        return _page.FocusAsync(selector);
    }

    public Task<string> GetContentAsync()
    {
        return _page.GetContentAsync();
    }

    public Task<CookieParam[]> GetCookiesAsync(params string[] urls)
    {
        return _page.GetCookiesAsync(urls);
    }

    public Task<string> GetTitleAsync()
    {
        return _page.GetTitleAsync();
    }

    public Task<IResponse> GoBackAsync(NavigationOptions? options = null)
    {
        return _page.GoBackAsync(options);
    }

    public Task<IResponse> GoForwardAsync(NavigationOptions? options = null)
    {
        return _page.GoForwardAsync(options);
    }

    public Task HoverAsync(string selector)
    {
        return _page.HoverAsync(selector);
    }

    public Task<Dictionary<string, decimal>> MetricsAsync()
    {
        return _page.MetricsAsync();
    }

    public Task PdfAsync(string file)
    {
        return _page.PdfAsync(file);
    }

    public Task PdfAsync(string file, PdfOptions options)
    {
        return _page.PdfAsync(file, options);
    }

    public Task<byte[]> PdfDataAsync()
    {
        return _page.PdfDataAsync();
    }

    public Task<byte[]> PdfDataAsync(PdfOptions options)
    {
        return _page.PdfDataAsync(options);
    }

    public Task<Stream> PdfStreamAsync()
    {
        return _page.PdfStreamAsync();
    }

    public Task<Stream> PdfStreamAsync(PdfOptions options)
    {
        return _page.PdfStreamAsync(options);
    }

    public Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
    {
        return _page.QueryObjectsAsync(prototypeHandle);
    }

    public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
    {
        return _page.QuerySelectorAllAsync(selector);
    }

    public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
    {
        return _page.QuerySelectorAllHandleAsync(selector);
    }

    public Task<IElementHandle> QuerySelectorAsync(string selector)
    {
        return _page.QuerySelectorAsync(selector);
    }

    public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[]? waitUntil = null)
    {
        return _page.ReloadAsync(timeout, waitUntil);
    }

    public Task<IResponse> ReloadAsync(NavigationOptions options)
    {
        return _page.ReloadAsync(options);
    }

    public Task ScreenshotAsync(string file)
    {
        return _page.ScreenshotAsync(file);
    }

    public Task ScreenshotAsync(string file, ScreenshotOptions options)
    {
        return _page.ScreenshotAsync(file, options);
    }

    public Task<string> ScreenshotBase64Async()
    {
        return _page.ScreenshotBase64Async();
    }

    public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
    {
        return _page.ScreenshotBase64Async(options);
    }

    public Task<byte[]> ScreenshotDataAsync()
    {
        return _page.ScreenshotDataAsync();
    }

    public Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
    {
        return _page.ScreenshotDataAsync(options);
    }

    public Task<Stream> ScreenshotStreamAsync()
    {
        return _page.ScreenshotStreamAsync();
    }

    public Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
    {
        return _page.ScreenshotStreamAsync(options);
    }

    public Task<string[]> SelectAsync(string selector, params string[] values)
    {
        return _page.SelectAsync(selector, values);
    }

    public Task SetBurstModeOffAsync()
    {
        return _page.SetBurstModeOffAsync();
    }

    public Task SetBypassCSPAsync(bool enabled)
    {
        return _page.SetBypassCSPAsync(enabled);
    }

    public Task SetCacheEnabledAsync(bool enabled = true)
    {
        return _page.SetCacheEnabledAsync(enabled);
    }

    public Task SetContentAsync(string html, NavigationOptions? options = null)
    {
        return _page.SetContentAsync(html, options);
    }

    public Task SetCookieAsync(params CookieParam[] cookies)
    {
        return _page.SetCookieAsync(cookies);
    }

    public Task SetDragInterceptionAsync(bool enabled)
    {
        return _page.SetDragInterceptionAsync(enabled);
    }

    public Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
    {
        return _page.SetExtraHttpHeadersAsync(headers);
    }

    public Task SetGeolocationAsync(GeolocationOption options)
    {
        return _page.SetGeolocationAsync(options);
    }

    public Task SetJavaScriptEnabledAsync(bool enabled)
    {
        return _page.SetJavaScriptEnabledAsync(enabled);
    }

    public Task SetOfflineModeAsync(bool value)
    {
        return _page.SetOfflineModeAsync(value);
    }

    public Task SetRequestInterceptionAsync(bool value)
    {
        return _page.SetRequestInterceptionAsync(value);
    }

    public Task SetUserAgentAsync(string userAgent, UserAgentMetadata? userAgentData = null)
    {
        return _page.SetUserAgentAsync(userAgent, userAgentData);
    }

    public Task SetViewportAsync(ViewPortOptions viewport)
    {
        return _page.SetViewportAsync(viewport);
    }

    public Task TapAsync(string selector)
    {
        return _page.TapAsync(selector);
    }

    public Task TypeAsync(string selector, string text, TypeOptions? options = null)
    {
        return _page.TypeAsync(selector, text, options);
    }

    public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions? options = null)
    {
        return _page.WaitForExpressionAsync(script, options);
    }

    public Task<FileChooser> WaitForFileChooserAsync(WaitForFileChooserOptions? options = null)
    {
        return _page.WaitForFileChooserAsync(options);
    }

    public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForFrameAsync(url, options);
    }

    public Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForFrameAsync(predicate, options);
    }

    public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions? options = null, params object[] args)
    {
        return _page.WaitForFunctionAsync(script, options, args);
    }

    public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args)
    {
        return _page.WaitForFunctionAsync(script, args);
    }

    public Task<IResponse> WaitForNavigationAsync(NavigationOptions? options = null)
    {
        return _page.WaitForNavigationAsync(options);
    }

    public Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions? options = null)
    {
        return _page.WaitForNetworkIdleAsync(options);
    }

    public Task<PuppeteerSharp.IRequest> WaitForRequestAsync(Func<PuppeteerSharp.IRequest, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForRequestAsync(predicate, options);
    }

    public Task<PuppeteerSharp.IRequest> WaitForRequestAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForRequestAsync(url, options);
    }

    public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(predicate, options);
    }

    public Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(predicate, options);
    }

    public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions? options = null)
    {
        return _page.WaitForResponseAsync(url, options);
    }

    public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions? options = null)
    {
        return _page.WaitForSelectorAsync(selector, options);
    }

    public Task WaitForTimeoutAsync(int milliseconds)
    {
        return _page.WaitForTimeoutAsync(milliseconds);
    }

    public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions? options = null)
    {
        return _page.WaitForXPathAsync(xpath, options);
    }

    public Task<IElementHandle[]> XPathAsync(string expression)
    {
        return _page.XPathAsync(expression);
    }

    #endregion
}
