namespace GreatJorb.Tests.Features;

[Category(TestType.UnitTest)]
public class TryParsePropertyFromTextQueryTests
{

    [TestCase("Contractor", nameof(JobPosting.JobType), JobType.Contract)]
    [TestCase("Contract", nameof(JobPosting.JobType), JobType.Contract)]
    [TestCase("Part Time", nameof(JobPosting.JobType), JobType.PartTime)]
    [TestCase("Part-Time", nameof(JobPosting.JobType), JobType.PartTime)]
    [TestCase("Remote", nameof(JobPosting.WorkplaceType), WorkplaceType.Remote)]
    [TestCase("Work from Home", nameof(JobPosting.WorkplaceType), WorkplaceType.Remote)]
    [TestCase("work from home", nameof(JobPosting.WorkplaceType), WorkplaceType.Remote)]

    public async Task CanParseProperty(string text, string expectedProperty, object expectedValue)
    {
        var serviceProvider = TestServiceProvider
            .CreateServiceProvider(includeMediator: true);

        var result = await serviceProvider.Mediator.Send(new TryParsePropertyFromTextQuery(text));

        Assert.AreEqual(expectedProperty, result[0].JobInfoProperty.Name);
        Assert.AreEqual(expectedValue.ToString(), result[0].ParsedValue.ToString());
    }

    [TestCase("Mud Shoveler 250K to 300k", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler $250K to $300k", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler $250K-$300k", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler $250K–$300k", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler $250K—$300k", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler 250–300K", 250000.0, 300000.0)]
    [TestCase("Mud Shoveler 250 to 300K", 250000.0, 300000.0)]
    public async Task CanParseSalary(string text, decimal min, decimal max)
    {
        var serviceProvider = TestServiceProvider
            .CreateServiceProvider(includeMediator: true);

        var result = await serviceProvider.Mediator.Send(new TryParsePropertyFromTextQuery(text));

        var minResult = result.FirstOrDefault(p => p.JobInfoProperty.Name == nameof(JobPosting.SalaryMin));

        Assert.IsNotNull(minResult);
        Assert.AreEqual(min, minResult!.ParsedValue);

        var maxResult = result.FirstOrDefault(p => p.JobInfoProperty.Name == nameof(JobPosting.SalaryMax));
        Assert.IsNotNull(maxResult);
        Assert.AreEqual(max, maxResult!.ParsedValue);
    }
}
