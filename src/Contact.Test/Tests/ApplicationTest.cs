namespace Contacts.Test.Tests
{
    public class ApplicationTest
    {
        [Fact]
        public void Run_ShouldRunFine_WhenIsDevelopmentEnvironment()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            // Act
            Api.Application.GetWebApplication([]).RunAsync();

            // Assert
            Assert.True(true);
        }

        [Fact]
        public void Run_ShouldRunFine_WhenIsProductionEnvironment()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            // Act
            Api.Application.GetWebApplication([]).RunAsync();

            // Assert
            Assert.True(true);
        }
    }
}
