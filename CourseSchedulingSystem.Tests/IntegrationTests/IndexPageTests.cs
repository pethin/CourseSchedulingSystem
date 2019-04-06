using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Tests.Factories;
using CourseSchedulingSystem.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CourseSchedulingSystem.Tests.IntegrationTests
{
    public class IndexPageTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly TestWebApplicationFactory<Startup> _factory;

        public IndexPageTests(TestWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        public async Task Get_RedirectsToLoginIfNotAuthenticated(string url)
        {
            // Act
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.StartsWith("/Identity/Account/Login",
                response.RequestMessage.RequestUri.PathAndQuery);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        public async Task Get_RedirectsToManageIfAuthenticated(string url)
        {
            // Act
            var user = new UserFactory().Generate();

            // CreateClient instantiates _factory.Server
            var client = _factory.CreateClient();
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Add(user);
                await dbContext.SaveChangesAsync();
            }

            await client.ActAsAsync(user.UserName);

            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.StartsWith("/Manage",
                response.RequestMessage.RequestUri.PathAndQuery);
        }
    }
}