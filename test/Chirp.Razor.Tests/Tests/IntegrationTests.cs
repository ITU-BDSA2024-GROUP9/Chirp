using Chirp.Repositories.Repositories;
using Chirp.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests.Tests
{
	// This class was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
	public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<InMemoryDatabaseHelper>
	{
		private readonly WebApplicationFactory<Program> _fixture;
		private readonly HttpClient _client;
		private readonly InMemoryDatabaseHelper _testDatabaseFixture;

		// The constructor should accept both WebApplicationFactory<Program> and TestDatabaseFixture
		public IntegrationTests(WebApplicationFactory<Program> fixture, InMemoryDatabaseHelper testDatabaseFixture)
		{
			_testDatabaseFixture = testDatabaseFixture;

			// Customize the web host to use the test-specific configuration
			_fixture = fixture.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					// Remove the existing DbContext registration (if it exists)
					var descriptor = services.SingleOrDefault(
						d => d.ServiceType == typeof(DbContextOptions<ChirpDBContext>));
					if (descriptor != null)
					{
						services.Remove(descriptor);
					}

					// Register the test-specific DbContext using the fixture's connection string
					services.AddDbContext<ChirpDBContext>(options =>
					{
						options.UseSqlite(_testDatabaseFixture.ConnectionString);
					});

					// Ensure migrations are applied before tests run
					using (var scope = services.BuildServiceProvider().CreateScope())
					{
						var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
						dbContext.Database.Migrate();  // Apply migrations if necessary
					}
				});
			});

			// Initialize HttpClient
			_client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = true,
				HandleCookies = true
			});
		}


		// This test is from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
		[Xunit.Theory]
		[InlineData("/")]
		public async Task Get_EndpointsReturnSuccess(string url)
		{
			// Act
			var response = await _client.GetAsync(url);

			// Assert
			response.EnsureSuccessStatusCode(); // Status Code 200-299
		}

		// This was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
		[Xunit.Theory]
		[InlineData("Jacqualine Gilcoine")]
		[InlineData("Quintin Sitts")]
		public async Task CanSeePrivateTimelineAzure(string author)
		{
			var response = await _client.GetAsync($"/{author}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			//Assert.Fail(content);
			Assert.Contains("Chirp!", content);
			Assert.Contains($"{author}'s Timeline", content);

		}

		[Fact]
		public async Task CanSeePublicTimeline()
		{
			var response = await _client.GetAsync("/");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();

			Assert.Contains("Chirp!", content);
			Assert.Contains("Public Timeline", content);
		}


		static Int32 SubstringCount(string orig, string find)
		{
			var s2 = orig.Replace(find, "");
			return (orig.Length - s2.Length) / find.Length;
		}

		[Fact]
		public async Task PagesLimitedTo32()
		{
			// arrange
			var response = await _client.GetAsync("/");
			response.EnsureSuccessStatusCode();

			// act
			var content = await response.Content.ReadAsStringAsync();

			// assert
			Assert.Contains("Chirp!", content);
			Assert.Equal(32, SubstringCount(content, "<p>"));

		}

		[Xunit.Theory]
		[InlineData("Jacqualine Gilcoine")]
		[InlineData("Quintin Sitts")]
		public async Task CanSeePrivateTimeline(string author)
		{
			var response = await _client.GetAsync($"/{author}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			Assert.Contains("Chirp!", content);
			Assert.Contains($"{author}'s Timeline", content);
		}

		[Xunit.Theory]
		[InlineData("Jacqualine Gilcoine")]
		[InlineData("Quintin Sitts")]
		public async Task CanSeePrivateTimelineFromName(string author)
		{
			var response = await _client.GetAsync($"/{author}");
			response.EnsureSuccessStatusCode();
			var content = await response.Content.ReadAsStringAsync();
			Assert.Contains("Chirp!", content);
			Assert.Contains($"{author}'s Timeline", content);
		}
	}
}
