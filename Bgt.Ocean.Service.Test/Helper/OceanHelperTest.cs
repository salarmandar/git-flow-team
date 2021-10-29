using Bgt.Ocean.Infrastructure.Helpers;
using Xunit;

namespace Bgt.Ocean.Service.Test.Helper
{
    public class OceanHelperTest
    {

        [Theory]
        [InlineData(null, 0, null)]
        [InlineData("", 0, "")]
        [InlineData("1234", 0, "")]
        [InlineData("1234", 1, "1")]
        [InlineData("1234", 3, "123")]
        [InlineData("1234", 4, "1234")]
        [InlineData("1234", 10, "1234")]
        public void Truncate_Pass(string value, int length, string expected)
        {
            var result = value.Truncate(length);
            Assert.Equal(expected, result);
        }
    }
}
