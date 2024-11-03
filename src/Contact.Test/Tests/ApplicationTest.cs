namespace Contacts.Test.Tests
{
    public class ApplicationTest
    {
        [Fact]
        public async Task Run_ShouldRunFine_WhenIsDevelopmentEnvironment()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            // Act
            await Api.Application.Run([]);

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Run_ShouldRunFine_WhenIsProductionEnvironment()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

            // Act
            await Api.Application.Run([]);

            // Assert
            Assert.True(true);
        }
    }
}
