using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMillionRecords.Utilities;

namespace AMillionRecords.Test
{
    [TestClass]
    public class RecordGeneratorTests
    {
        [TestMethod]
        public void GenerateRecord()
        {
            var sut = new RandomRecordGenerator();
            var result = sut.GenerateRandomRecord();
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result.CustomerName));
            Assert.IsInstanceOfType(result.AccountId, typeof(Guid));
        }

        [TestMethod]
        public void GenerateRecords_ReturnsListOfRecords()
        {
            var sut = new RandomRecordGenerator();
            int numRecords = 1000;
            var result = sut.GenerateRandomRecords(numRecords);
            Assert.IsTrue(result.Count == numRecords);

        }
    }
}
