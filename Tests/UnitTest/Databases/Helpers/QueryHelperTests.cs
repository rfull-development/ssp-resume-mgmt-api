// Copyright (c) 2024 RFull Development
// This source code is managed under the MIT license. See LICENSE in the project root.
using ResumeManagementApi.Databases.Exceptions;
using ResumeManagementApi.Databases.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitTest.Databases.Helpers
{
    [TestClass()]
    [TestCategory("Unit")]
    public class QueryHelperTests
    {
        private TargetModel _model = null!;
        private TargetInvalidModel _invalidModel = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _model = new TargetModel
            {
                Id = 1,
                Name = "test",
            };
            _invalidModel = new TargetInvalidModel
            {
                Id = 1,
                Name = "test",
            };
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void GetPropertyNameListTest()
        {
            List<string> names = QueryHelper.GetPropertyNameList(_model);
            Assert.AreEqual(2, names.Count);
            Assert.AreEqual("Id", names[0]);
            Assert.AreEqual("Name", names[1]);
        }

        [TestMethod]
        public void GetPropertyNameListTest_Exclude()
        {
            List<string> names = QueryHelper.GetPropertyNameList(_model, [
                "Id"
            ]);
            Assert.AreEqual(1, names.Count);
            Assert.AreEqual("Name", names[0]);
        }

        [TestMethod]
        public void GetPropertyNameListAllTest()
        {
            List<string> names = QueryHelper.GetPropertyNameList<TargetModel>();
            Assert.AreEqual(2, names.Count);
            Assert.AreEqual("Id", names[0]);
            Assert.AreEqual("Name", names[1]);
        }

        [TestMethod]
        public void GetColumnNameListTest()
        {
            List<string> names = QueryHelper.GetColumnNameList(_model);
            Assert.AreEqual(2, names.Count);
            Assert.AreEqual("id", names[0]);
            Assert.AreEqual("name", names[1]);
        }

        [TestMethod]
        public void GetColumnNameListTest_Exclude()
        {
            List<string> names = QueryHelper.GetColumnNameList(_model, [
                "Id"
            ]);
            Assert.AreEqual(1, names.Count);
            Assert.AreEqual("name", names[0]);
        }

        [TestMethod]
        public void GetColumnNameListTest_InvalidModel()
        {
            try
            {
                List<string> names = QueryHelper.GetColumnNameList(_invalidModel);
                Assert.Fail();
            }
            catch (DatabaseException)
            {
            }
        }

        [TestMethod]
        public void GetColumnNameListTest_Generic()
        {
            List<string> names = QueryHelper.GetColumnNameList<TargetModel>();
            Assert.AreEqual(2, names.Count);
            Assert.AreEqual("id", names[0]);
            Assert.AreEqual("name", names[1]);
        }

        [TestMethod]
        public void GetColumnListTest()
        {
            List<KeyValuePair<string, string>> columns = QueryHelper.GetColumnList(_model, [
                "Id"
                ]);
            Assert.AreEqual(1, columns.Count);
            Assert.AreEqual("name", columns[0].Key);
            Assert.AreEqual("Name", columns[0].Value);
        }

        [TestMethod]
        public void GetColumnListTest_InvalidModel()
        {
            try
            {
                List<KeyValuePair<string, string>> columns = QueryHelper.GetColumnList(_invalidModel);
                Assert.Fail();
            }
            catch (DatabaseException)
            {
            }
        }

        private class TargetModel
        {
            [Column("id")]
            public int Id { get; set; }
            [Column("name")]
            public string? Name { get; set; }
        }

        private class TargetInvalidModel
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
