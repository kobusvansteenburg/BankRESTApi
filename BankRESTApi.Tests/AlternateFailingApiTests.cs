using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using BankRESTApi;
using com.example.demo.models;
using System.Collections.Generic;

namespace BankRESTApi.Tests
{
    // Intentionally failing API tests that mirror CustomerApiTests but assert incorrect expectations
    public class AlternateFailingApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AlternateFailingApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Fail_GetAll_ShouldBeEmpty()
        {
            var res = await _client.GetAsync("/api/customers");
            res.EnsureSuccessStatusCode();
            var items = await res.Content.ReadFromJsonAsync<List<Customer>>();
            // intentionally wrong: expect empty when it should not be
            Assert.Empty(items);
        }

        [Fact]
        public async Task Fail_GetById_ShouldReturnNotFound_ForExisting()
        {
            var res = await _client.GetAsync("/api/customers/1");
            // intentionally wrong: expect NotFound even though it should exist
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Fail_CreateCustomer_ShouldReturnIdZero()
        {
            var newCustomer = new Customer { FirstName = "Fail", LastName = "Api", Email = "fail@api.test" };
            var res = await _client.PostAsJsonAsync("/api/customers", newCustomer);
            // intentionally assert Created but id equals 0
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            var c = await res.Content.ReadFromJsonAsync<Customer>();
            Assert.NotNull(c);
            Assert.Equal(0, c.Id);
        }

        [Fact]
        public async Task Fail_GetById_ReturnsDifferentId()
        {
            var res = await _client.GetAsync("/api/customers/1");
            res.EnsureSuccessStatusCode();
            var c = await res.Content.ReadFromJsonAsync<Customer>();
            // intentionally wrong expectation: expect id 999
            Assert.NotNull(c);
            Assert.Equal(999, c.Id);
        }
    }
}
