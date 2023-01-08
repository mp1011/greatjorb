namespace GreatJorb.Tests.Features;

[Category(TestType.UnitTest)]
public class SetPropertyFromTextCommandTests
{
    [Test]
    public async Task CanSetSalaryFromText()
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeMediator: true);

        var posting = new JobPosting();

        await serviceProvider.Mediator.Send(new SetPropertiesFromTextCommand(posting, "100K-200K"));
        Assert.AreEqual(posting.SalaryMin.GetValueOrDefault(), 100000.0);
    }
}
