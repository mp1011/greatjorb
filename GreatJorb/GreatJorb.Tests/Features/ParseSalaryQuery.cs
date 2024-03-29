﻿namespace GreatJorb.Tests.Features;


[Category(TestType.UnitTest)]
public class ParseSalaryQueryTests
{
    [TestCase("$128,505/yr - $183,575/yr", 128505, 183575, SalaryType.Annual)]
    [TestCase("$70,700/yr - $170,000/yr (from job description)", 70700, 170000, SalaryType.Annual)]
    [TestCase("70–75 an hour", 70, 75, SalaryType.Hourly)]
    [TestCase("130K–150K a year", 130000, 150000, SalaryType.Annual)]
    [TestCase("$140,000/yr - $200,000/yr (from job description)", 140000, 200000, SalaryType.Annual)]
    [TestCase("$65.00 - $73.01 an hour", 65,73, SalaryType.Hourly)]
    [TestCase("USD75 - USD85", 75,85, SalaryType.Hourly)]
    [TestCase("contract position is $75-$85/hour. Our salary ranges", 75,85, SalaryType.Hourly)]
    public async Task CanParseSalary(string text, decimal min, decimal max, SalaryType salaryType)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);
        var result = await serviceProvider.Mediator.Send(new ParseSalaryQuery(text));

        Assert.AreEqual(min, result.Min);
        Assert.AreEqual(max, result.Max);
        Assert.AreEqual(salaryType, result.SalaryType);
    }

    [TestCase("10,001+ employees · Financial Services")]
    [TestCase("51-200 employees · Software Development")]
    [TestCase("12 hours ago")]
    public async Task TextIsNotParsedAsSalary(string text)
    {
        using var serviceProvider = TestServiceProvider.CreateServiceProvider(includeMediator: true);
        var result = await serviceProvider.Mediator.Send(new ParseSalaryQuery(text));

        Assert.IsNull( result.Min);
        Assert.IsNull( result.Max);
        Assert.AreEqual(SalaryType.Unknown, result.SalaryType);
    }
}
