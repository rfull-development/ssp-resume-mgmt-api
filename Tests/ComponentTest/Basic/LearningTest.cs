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
    public class LearningTest
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
        public async Task CreateLearning()
        {
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning {Guid.NewGuid()}"
                };
                var response = await _client.Learnings.PostAsync(request);
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
        public async Task GetLearningList()
        {
            try
            {
                var response = await _client.Learnings.GetAsLearningListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if ((response.Count is not int count) ||
                    (response.Learnings is not List<Learning> learnings))
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(count, learnings.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetLearning()
        {
            string id;
            string summary;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning {Guid.NewGuid()}"
                };
                var response = await _client.Learnings.PostAsync(request);
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
                var response = await _client.Learnings[id].GetAsLearningGetResponseAsync();
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
        public async Task GetLearning_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Learnings[id].GetAsLearningGetResponseAsync();
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task UpdateLearning()
        {
            string id;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning {Guid.NewGuid()}"
                };
                var response = await _client.Learnings.PostAsync(request);
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
                LearningUpdateRequest request = new()
                {
                    Description = $"Test Learning {Guid.NewGuid()}"
                };
                await _client.Learnings[id].PatchAsync(request);
                description = request.Description;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Learnings[id].GetAsLearningGetResponseAsync();
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
        public async Task UpdateLearning_Overwrite()
        {
            string id;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning {Guid.NewGuid()}"
                };
                var response = await _client.Learnings.PostAsync(request);
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
                LearningUpdateRequest request = new()
                {
                    Description = $"Test Learning {Guid.NewGuid()}"
                };
                await _client.Learnings[id].PatchAsync(request);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string description;
            try
            {
                LearningUpdateRequest request = new()
                {
                    Description = $"Test Learning {Guid.NewGuid()}"
                };
                await _client.Learnings[id].PatchAsync(request);
                description = request.Description;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Learnings[id].GetAsLearningGetResponseAsync();
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
        public async Task UpdateLearning_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                LearningUpdateRequest request = new()
                {
                    Description = $"Test Learning {Guid.NewGuid()}"
                };
                await _client.Learnings[id].PatchAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteLearning()
        {
            string id;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning {Guid.NewGuid()}"
                };
                var response = await _client.Learnings.PostAsync(request);
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
                await _client.Learnings[id].DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Learnings[id].GetAsLearningGetResponseAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteLearning_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Learnings[id].DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }
    }
}
