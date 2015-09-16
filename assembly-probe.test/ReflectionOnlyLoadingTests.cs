namespace assembly_probe.test
{
    using Apprenda.Integrations.Inspection;

    using CustomAttributeTestTarget;

    using log4net.Config;

    using Xunit;

    /// <summary>
    /// Tests for the AssemblyExtensions which use Mono.Cecil to inspect assemblies.
    /// </summary>
    public class ReflectionOnlyLoadingTests
    {
        [Fact]
        public void CanFindXmlConfiguratorAttribute()
        {
            var configFileValue = AssemblyExtensions.GetAssemblyAttributePropertyValue<XmlConfiguratorAttribute, string>("aspnet-log4net-workload-assy-attribute.dll", "ConfigFile");
            Assert.Equal("default.log4net", configFileValue);
        }

        [Fact]
        public void CannotFindAbsentAttribute()
        {
            var descriptionValue =
    AssemblyExtensions.GetAssemblyAttributePropertyValue<CustomAssemblyAttribute, string>(
        "aspnet-log4net-workload-assy-attribute.dll", 
        "Description");
            Assert.Null(descriptionValue);
        }
    }
}
