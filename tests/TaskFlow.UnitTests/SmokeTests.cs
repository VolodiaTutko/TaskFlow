
using Xunit;

namespace TaskFlow.UnitTests;

public class SmokeTests
{
    [Fact]
    public void TestInfrastructure_ShouldRunSuccessfully()
    {
        int expected = 4;

        int actual = 2 + 2;

        Assert.Equal(expected, actual);
    }
}
