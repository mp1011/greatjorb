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
    [TestCase("Senior Software Engineer, Backend (Remote-Eligible)", nameof(JobPosting.WorkplaceType), WorkplaceType.Remote)]
    [TestCase("Senior Software Engineer, Backend (Remote)", nameof(JobPosting.WorkplaceType), WorkplaceType.Remote)]
    public async Task CanParseProperty(string text, string expectedProperty, object expectedValue)
    {
        using var serviceProvider = TestServiceProvider
            .CreateServiceProvider(includeMediator: true);

        var result = await serviceProvider.Mediator.Send(new TryParsePropertyFromTextQuery(text));

        Assert.AreEqual(expectedProperty, result[0].JobInfoProperty.Name);
        Assert.AreEqual(expectedValue.ToString(), result[0].ParsedValue.ToString());
    }

    [TestCase("Mud Shoveler 250K to 300k", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler $250K to $300k", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler $250K-$300k", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler $250K–$300k", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler $250K—$300k", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler 250–300K", 250000.0, 300000.0, null)]
    [TestCase("Mud Shoveler 250 to 300K", 250000.0, 300000.0, null)]
    [TestCase("$70,700/yr - $170,000/yr (from job description)", 70700.0, 170000.0, SalaryType.Annual)]
    [TestCase("70–75 an hour", 70, 75, SalaryType.Hourly)]
    [TestCase("130K–150K a year", 130000, 150000, SalaryType.Annual)]
    public async Task CanParseSalary(string text, decimal min, decimal max, SalaryType? salaryType)
    {
        using var serviceProvider = TestServiceProvider
            .CreateServiceProvider(includeMediator: true);

        var result = await serviceProvider.Mediator.Send(new TryParsePropertyFromTextQuery(text));

        var minResult = result.FirstOrDefault(p => p.JobInfoProperty.Name == nameof(JobPosting.SalaryMin));

        Assert.IsNotNull(minResult);
        Assert.AreEqual(min, minResult!.ParsedValue);

        var maxResult = result.FirstOrDefault(p => p.JobInfoProperty.Name == nameof(JobPosting.SalaryMax));
        Assert.IsNotNull(maxResult);
        Assert.AreEqual(max, maxResult!.ParsedValue);

        if(salaryType != null)
        {
            var salaryResult = result.FirstOrDefault(p => p.JobInfoProperty.Name == nameof(JobPosting.SalaryType));
            Assert.IsNotNull(salaryResult);
            Assert.AreEqual(salaryType, salaryResult!.ParsedValue);
        }
    }
}
