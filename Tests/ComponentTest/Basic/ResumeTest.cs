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
    public class ResumeTest
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
        public async Task CreateResume()
        {
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task CreateResume_Duplicate()
        {
            string id;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                id = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = id
                };
                await _client.Resumes.PostAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task GetResumeList()
        {
            try
            {
                var response = await _client.Resumes.GetAsResumeListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if ((response.Count is not int count) ||
                    (response.Resumes is not List<Resume> resumes))
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(count, resumes.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetResume()
        {
            string id;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                id = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[id].GetAsResumeGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                Assert.AreEqual(id, response.Id);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetResume_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Resumes[id].GetAsResumeGetResponseAsync();
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteResume()
        {
            string id;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                id = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                await _client.Resumes[id].DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Resumes[id].GetAsResumeGetResponseAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteResume_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Resumes[id].DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task GetSkillList()
        {
            string id;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                id = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[id].Skills.GetAsSkillListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if ((response.Count is not int) ||
                    (response.Skills is not List<Skill> resumes))
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(0, resumes.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task CreateSkill()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
                {
                    Assert.Fail();
                }
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetSkill()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            string skillId;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                id = response.Id;
                skillId = request.SkillId;

            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].GetAsSkillGetResponseAsync();
                if (string.IsNullOrEmpty(response?.SkillId))
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                Assert.AreEqual(skillId, response.SkillId);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task GetSkill_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                string skillId = "InvalidId";
                await _client.Resumes[id].Skills[skillId].GetAsSkillGetResponseAsync();
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task UpdateSkill()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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

            string newSkillId;
            try
            {
                SkillUpdateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}"
                };
                await _client.Resumes[resumeId].Skills[id].PatchAsync(request);
                newSkillId = request.SkillId;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].GetAsSkillGetResponseAsync();
                if (string.IsNullOrEmpty(response?.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(newSkillId, response.SkillId);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateSkill_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                string skillId = "InvalidId";
                SkillUpdateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                await _client.Resumes[id].Skills[skillId].PatchAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteSkill()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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
                await _client.Resumes[resumeId].Skills[id].DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Resumes[resumeId].Skills[id].GetAsSkillGetResponseAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteSkill_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                string skillId = "InvalidId";
                await _client.Resumes[id].Skills[skillId].DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task GetSkillUsageList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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
                var response = await _client.Resumes[resumeId].Skills[id].Usages.GetAsSkillUsageListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if (response.Count is not int)
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(0, response.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateSkillUsageList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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

            string usageId;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage-{Guid.NewGuid()}",
                };
                var response = await _client.Usages.PostAsync(request);
                if ((response is null) ||
                    string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                usageId = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            List<SkillUsageUpdate> exceptedUsages;
            try
            {
                List<SkillUsageUpdate> usages = [
                    new SkillUsageUpdate
                    {
                        Id = usageId,
                        Description = $"Test Usage-{Guid.NewGuid()}",
                    }
                ];
                SkillUsageListUpdateRequest request = new()
                {
                    Usages = usages
                };
                await _client.Resumes[resumeId].Skills[id].Usages.PutAsync(request);
                exceptedUsages = usages;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].Usages.GetAsSkillUsageListGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(exceptedUsages.Count, response.Count);
                var usages = response.Usages;
                if (usages is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                foreach (var usage in usages)
                {
                    bool found = false;
                    foreach (var exceptedUsage in exceptedUsages)
                    {
                        if ((usage.Id == exceptedUsage.Id) &&
                            (usage.Description == exceptedUsage.Description))
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.IsTrue(found);
                }
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateSkillUsageList_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                SkillUsageListUpdateRequest request = new()
                {
                    Usages = [
                        new SkillUsageUpdate
                        {
                            Id = "InvalidId",
                            Description = $"Test Usage-{Guid.NewGuid()}",
                        }
                    ]
                };
                await _client.Resumes[id].Skills[id].Usages.PutAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteSkillUsageList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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

            string usageId;
            try
            {
                UsageCreateRequest request = new()
                {
                    Summary = $"Test Usage-{Guid.NewGuid()}",
                };
                var response = await _client.Usages.PostAsync(request);
                if ((response is null) ||
                    string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                usageId = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                List<SkillUsageUpdate> usages = [
                    new SkillUsageUpdate
                    {
                        Id = usageId,
                        Description = $"Test Usage-{Guid.NewGuid()}",
                    }
                ];
                SkillUsageListUpdateRequest request = new()
                {
                    Usages = usages
                };
                await _client.Resumes[resumeId].Skills[id].Usages.PutAsync(request);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Resumes[resumeId].Skills[id].Usages.DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].Usages.GetAsSkillUsageListGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(0, response.Count);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task DeleteSkillUsageList_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Resumes[id].Skills[id].Usages.DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task GetSkillLearningList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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
                var response = await _client.Resumes[resumeId].Skills[id].Learnings.GetAsSkillLearningListGetResponseAsync();
                if (response is null)
                {
                    Assert.Fail("Failed to get response.");
                    return;
                }
                if (response.Count is not int)
                {
                    Assert.Fail(response.ToString());
                    return;
                }
                Assert.AreEqual(0, response.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateSkillLearningList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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

            string learningId;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning-{Guid.NewGuid()}",
                };
                var response = await _client.Learnings.PostAsync(request);
                if ((response is null) ||
                    string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                learningId = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            List<SkillLearningUpdate> exceptedLearnings;
            try
            {
                List<SkillLearningUpdate> learnings = [
                    new SkillLearningUpdate
                    {
                        Id = learningId,
                        Description = $"Test Learning-{Guid.NewGuid()}",
                    }
                ];
                SkillLearningListUpdateRequest request = new()
                {
                    Learnings = learnings
                };
                await _client.Resumes[resumeId].Skills[id].Learnings.PutAsync(request);
                exceptedLearnings = learnings;
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].Learnings.GetAsSkillLearningListGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(exceptedLearnings.Count, response.Count);
                var learnings = response.Learnings;
                if (learnings is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                foreach (var learning in learnings)
                {
                    bool found = false;
                    foreach (var exceptedLearning in exceptedLearnings)
                    {
                        if ((learning.Id == exceptedLearning.Id) &&
                            (learning.Description == exceptedLearning.Description))
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.IsTrue(found);
                }
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task UpdateSkillLearningList_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                SkillLearningListUpdateRequest request = new()
                {
                    Learnings = [
                        new SkillLearningUpdate
                        {
                            Id = "InvalidId",
                            Description = $"Test Learning-{Guid.NewGuid()}",
                        }
                    ]
                };
                await _client.Resumes[id].Skills[id].Learnings.PutAsync(request);
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }

        [TestMethod]
        public async Task DeleteSkillLearningList()
        {
            string resumeId;
            try
            {
                ResumeCreateRequest request = new()
                {
                    UserId = Guid.NewGuid().ToString()
                };
                await _client.Resumes.PostAsync(request);
                resumeId = request.UserId;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            string id;
            try
            {
                SkillCreateRequest request = new()
                {
                    SkillId = $"Test Skill {Guid.NewGuid()}",
                };
                var response = await _client.Resumes[resumeId].Skills.PostAsync(request);
                if (string.IsNullOrEmpty(response?.Id))
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

            string learningId;
            try
            {
                LearningCreateRequest request = new()
                {
                    Summary = $"Test Learning-{Guid.NewGuid()}",
                };
                var response = await _client.Learnings.PostAsync(request);
                if ((response is null) ||
                    string.IsNullOrEmpty(response.Id))
                {
                    Assert.Inconclusive();
                    return;
                }
                learningId = response.Id;
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }

            try
            {
                List<SkillLearningUpdate> learnings = [
                    new SkillLearningUpdate
                    {
                        Id = learningId,
                        Description = $"Test Learning-{Guid.NewGuid()}",
                    }
                ];
                SkillLearningListUpdateRequest request = new()
                {
                    Learnings = learnings
                };
                await _client.Resumes[resumeId].Skills[id].Learnings.PutAsync(request);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                await _client.Resumes[resumeId].Skills[id].Learnings.DeleteAsync();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
                return;
            }

            try
            {
                var response = await _client.Resumes[resumeId].Skills[id].Learnings.GetAsSkillLearningListGetResponseAsync();
                if (response is null)
                {
                    Assert.Inconclusive();
                    return;
                }
                Assert.AreEqual(0, response.Count);
            }
            catch (Exception exception)
            {
                Assert.Inconclusive(exception.Message);
                return;
            }
        }

        [TestMethod]
        public async Task DeleteSkillLearningList_InvalidId()
        {
            try
            {
                string id = "InvalidId";
                await _client.Resumes[id].Skills[id].Learnings.DeleteAsync();
                Assert.Fail();
                return;
            }
            catch
            {
            }
        }
    }
}
