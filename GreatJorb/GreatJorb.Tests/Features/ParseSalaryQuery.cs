namespace GreatJorb.Tests.Features;


[Category(TestType.UnitTest)]
public class ParseSalaryQueryTests
{
    [TestCase("$128,505/yr - $183,575/yr", 128505, 183575, SalaryType.Annual)]
    [TestCase("$70,700/yr - $170,000/yr (from job description)", 70700, 170000, SalaryType.Annual)]
    [TestCase("70–75 an hour", 70, 75, SalaryType.Hourly)]
    [TestCase("130K–150K a year", 130000, 150000, SalaryType.Annual)]
    [TestCase("$140,000/yr - $200,000/yr (from job description)", 140000, 200000, SalaryType.Annual)]
    public async Task CanParseSalary(string text, decimal min, decimal max, SalaryType salaryType)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);
        var result = await serviceProvider.Mediator.Send(new ParseSalaryQuery(text));

        Assert.AreEqual(min, result.Min);
        Assert.AreEqual(max, result.Max);
        Assert.AreEqual(salaryType, result.SalaryType);
    }

    [TestCase("10,001+ employees · Financial Services")]
    [TestCase("51-200 employees · Software Development")]

    public async Task TextIsNotParsedAsSalary(string text)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);
        var result = await serviceProvider.Mediator.Send(new ParseSalaryQuery(text));

        Assert.IsNull( result.Min);
        Assert.IsNull( result.Max);
        Assert.AreEqual(SalaryType.Unknown, result.SalaryType);
    }
}
