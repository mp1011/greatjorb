namespace GreatJorb.Tests;

public class TestNotificationHandler : INotificationHandler<BrowserPageChanged>
{
    public Task Handle(BrowserPageChanged notification, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Browser Change Notification: " + notification);
        return Task.CompletedTask;
    }
}
