using PuppeteerSharp;
using System.Collections.Generic;

namespace GreatJorb.Tests.Features;

[Category(TestType.WebTest1)]
public class BrowseToPageQueryTests
{
    private List<string> _messages = new();

    [TestCase("https://www.monster.com")]
    public async Task CanBrowseToPage(string url)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(
           includeConfiguration: true,
           includeMediator: true,
           includePuppeteer: true);

        var result = await serviceProvider.Mediator.Send(new BrowseToPageQuery(url));
        Assert.NotNull(result);
    }

    private void Client_Disconnected(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("!");
    }

    private void Client_MessageReceived(object? sender, MessageEventArgs e)
    {
        if (e == null) return;

        _messages.Add(e.MessageData.ToString());
    }
}
