// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using Api.Client;
using Api.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace ComponentTest.Basic
{
    [TestClass]
    [TestCategory("Component")]
    public class UsageTest
    {
        private static IHttpClientFactory _httpClientFactory = null!;
        private ApiClient _client = null!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            ServiceCollection services = new();
            services.AddHttpClient();
            var provider = services.BuildServiceProvider();
            _httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            AnonymousAuthenticationProvider provider = new();
            HttpClientRequestAdapter adapter = new(authenticationProvider: provider, httpClient: httpClient);
            _client = new(adapter);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public async Task CreateUsage()
        {
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage {Guid.NewGuid()}"
                };
                var response = await _client.Usages.PostAsync(request);
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if (string.IsNullOrEmpty(response.Id))
                {
                    Assert.Fail(response.ToString());
                    return;
                }
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetUsageList()
        {
            try
            {
                var response = await _client.Usages.GetAsUsageListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if ((response.Count is not int count) ||
                    (response.Usages is not List<Usage> usages))
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(count, usages.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetUsage()
        {
            string id;
            string summary;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage {Guid.NewGuid()}"
                };
                var response = await _client.Usages.PostAsync(request);
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                if (string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                id = response.Id;
                summary = request.Summary;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Usages[id].GetAsUsageGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                Assert.AreEqual(id, response.Id);
                Assert.AreEqual(summary, response.Summary);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetUsage_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Usages[id].GetAsUsageGetResponseAsync();
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task UpdateUsage()
        {
            string id;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage {Guid.NewGuid()}"
                };
                var response = await _client.Usages.PostAsync(request);
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                if (string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                id = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string description;
            try
            {
                UsageUpdateRequest request = new()
                {
                    Description = $"Test Usage {Guid.NewGuid()}"
                };
                await _client.Usages[id].PatchAsync(request);
                description = request.Description;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Usages[id].GetAsUsageGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(description, response.Description);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateUsage_Overwrite()
        {
            string id;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage {Guid.NewGuid()}"
                };
                var response = await _client.Usages.PostAsync(request);
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                if (string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                id = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                UsageUpdateRequest request = new()
                {
                    Description = $"Test Usage {Guid.NewGuid()}"
                };
                await _client.Usages[id].PatchAsync(request);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string description;
            try
            {
                UsageUpdateRequest request = new()
                {
                    Description = $"Test Usage {Guid.NewGuid()}"
                };
                await _client.Usages[id].PatchAsync(request);
                description = request.Description;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Usages[id].GetAsUsageGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(description, response.Description);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateUsage_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                UsageUpdateRequest request = new()
                {
                    Description = $"Test Usage {Guid.NewGuid()}"
                };
                await _client.Usages[id].PatchAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteUsage()
        {
            string id;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage {Guid.NewGuid()}"
                };
                var response = await _client.Usages.PostAsync(request);
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                if (string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                id = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                await _client.Usages[id].DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Usages[id].GetAsUsageGetResponseAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteUsage_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Usages[id].DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }
    }
}
