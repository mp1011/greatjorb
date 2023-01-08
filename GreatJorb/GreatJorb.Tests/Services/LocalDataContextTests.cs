namespace GreatJorb.Tests.Services;

[Category(TestType.UnitTest)]
public class LocalDataContextTests
{
    public class TestModel : ILocalStorable
    {
        public string StorageKey { get; set; } = "";
        public string Test { get; set; } = "";
    }

    [Test]
    public async Task CanStoreDataLocally()
    {
        var serviceProvider = TestServiceProvider.CreateServiceProvider(
            includeConfiguration: true,
            includeDataContext: true,
            includeMediator: true);

        string guid = Guid.NewGuid().ToString();

        var testData1 = new TestModel()
        {
            StorageKey = "TEST123_" + guid,
            Test = guid,
        };

        var testData2 = new TestModel()
        {
            StorageKey = "TEST456_ " + guid,
            Test = guid,
        };

        using var context = serviceProvider.LocalDataContextProvider.GetContext();

        await context.Store(testData1);
        await context.Store(testData2);

        var retrieved = await context.Retrieve<TestModel>(p => p.Test == guid);
        Assert.AreEqual(2, retrieved.Length);

    }
}
