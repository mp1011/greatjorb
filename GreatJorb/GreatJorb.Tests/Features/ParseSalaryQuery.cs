namespace GreatJorb.Tests.Features;


[Category(TestType.UnitTest)]
public class ParseSalaryQueryTests
{
    [TestCase("$128,505/yr - $183,575/yr", 128505, 183575, SalaryType.Annual)]
    [TestCase("$70,700/yr - $170,000/yr (from job description)", 70700, 170000, SalaryType.Annual)]
    public async Task CanParseSalary(string text, decimal min, decimal max, SalaryType salaryType)
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);
        var result = await serviceProvider.Mediator.Send(new ParseSalaryQuery(text));

        Assert.AreEqual(min, result.Min);
        Assert.AreEqual(max, result.Max);
        Assert.AreEqual(salaryType, result.SalaryType);
    }
}
