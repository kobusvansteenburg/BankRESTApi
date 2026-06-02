using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using BankRESTApi;
using com.example.demo.models;
using System.Collections.Generic;

public class CustomerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CustomerApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessAndContent()
    {
        var res = await _client.GetAsync("/api/customers");
        res.EnsureSuccessStatusCode();
        var items = await res.Content.ReadFromJsonAsync<List<Customer>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items);
    }

    [Fact]
    public async Task GetById_ReturnsCustomer_WhenExists()
    {
        var res = await _client.GetAsync("/api/customers/1");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var c = await res.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(c);
        Assert.Equal(1, c.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var res = await _client.GetAsync("/api/customers/99999");
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreated()
    {
        var newCustomer = new Customer { FirstName = "Test", LastName = "User", Email = "test@example.com" };
        var res = await _client.PostAsJsonAsync("/api/customers", newCustomer);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var c = await res.Content.ReadFromJsonAsync<Customer>();
        Assert.NotNull(c);
        Assert.True(c.Id > 0);
    }
}
