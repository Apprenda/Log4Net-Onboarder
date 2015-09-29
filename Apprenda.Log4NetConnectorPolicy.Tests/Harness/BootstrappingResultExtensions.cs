using Apprenda.API.Extension.Bootstrapping;
using FluentAssertions;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    internal static class BootstrappingResultExtensions
    {
        public static void ShouldBeSuccessful(this BootstrappingResult result)
        {
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeNullOrEmpty();
        }
    }
}