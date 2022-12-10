using GreatJorb.Business.Extensions;

namespace GreatJorb.Tests.Extensions
{
    public class EnumParsingTests
    {
        [TestCase("On-site", WorkplaceType.OnSite)]
        [TestCase("Remote", WorkplaceType.Remote)]
        [TestCase("Hybrid", WorkplaceType.Hybrid)]
        public void CanParseWorkplaceType(string type, WorkplaceType expected)
        {
            TestParseEnum(type, expected);
        }

        [TestCase("Mid-Senior level", JobLevel.MidSeniorLevel)]
        public void CanParseJobLevel(string type, JobLevel expected)
        {
            TestParseEnum(type, expected);
        }

        private void TestParseEnum<T>(string text, T expected)
            where T:struct, Enum 
        {
            var parsed = text.TryParseEnumAdvanced(default(T));
            Assert.AreEqual(expected, parsed);
        }
    }
}
