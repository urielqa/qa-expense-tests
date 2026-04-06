using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Integration.Fixtures;

// Um HttpClient só pra turma de teste. URL: EXPENSE_API_BASE_URL, appsettings, ou localhost:5000.
public sealed class ExpenseApiFixture : IDisposable
{
    public HttpClient Client { get; }

    public ExpenseApiFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Integration.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var baseUrl = Environment.GetEnvironmentVariable("EXPENSE_API_BASE_URL")
            ?? config["Api:BaseUrl"]
            ?? "http://localhost:5000";

        var handler = new HttpClientHandler { AllowAutoRedirect = false };
        Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(30)
        };
        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Dispose() => Client.Dispose();
}
