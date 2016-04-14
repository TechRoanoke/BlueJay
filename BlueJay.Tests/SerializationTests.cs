using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BlueJay.Tests
{
    [TestClass]
    public class SerializationTests
    {
        class Wrapper
        {
            public Gender G { get; set; }
        }

        [TestMethod]
        public void ReadGender()
        {
            AssertGender(Gender.M, "M");
            AssertGender(Gender.M, "m"); // casing
            AssertGender(Gender.F, "F");
            AssertGender(Gender.U, "???"); // error case
        }

        [TestMethod]
        public void WriteGender()
        {

            var j1 = JsonConvert.SerializeObject(new Wrapper { G = Gender.M });
            Assert.AreEqual("{\"G\":\"M\"}", j1);

            var j2 = JsonConvert.SerializeObject(new Wrapper { G = Gender.F });
            Assert.AreEqual("{\"G\":\"F\"}", j2);

            var j3 = JsonConvert.SerializeObject(new Wrapper { G = Gender.U });
            Assert.AreEqual("{\"G\":\"U\"}", j3);
        }



        static void AssertGender(Gender genderExpected, string str)
        {
            var g = JsonConvert.DeserializeObject<Wrapper>("{ 'G' : '" + str + "'}");
            Assert.AreEqual(genderExpected, g.G);
        }
    }
}
