using System;
using System.Collections.Generic;
using NUnit.Framework;
using RecordTypeTable;

namespace RecordTypeTableTests
{
    public class Tests
    {
        List<FileCabinetRecord> records;
        private readonly List<FileCabinetRecord> nullList = null;

        [SetUp]
        public void Setup()
        {
            records = RecordGenerator.Generate(100000, 0);
        }

        [Test]
        public void GetTypeTableTimeTests_Console() //926 sec
        {
            records.GetTypeTable();
        }

        [Test]
        public void GetTypeTableTimeTests_File() //1 sec
        {
            records.GetTypeTable("records.txt");
        }

        [Test]
        public void GetTypeTableTests_IncorrectEnumerable() =>
            Assert.Throws<ArgumentNullException>((() => nullList.GetTypeTable()));
    }
}