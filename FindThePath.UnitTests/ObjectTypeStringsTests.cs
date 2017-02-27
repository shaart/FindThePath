using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindThePath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath.Tests
{
    [TestClass()]
    public class ObjectTypeStringsTests
    {
        [TestMethod()]
        [TestCategory("ObjectTypeStrings GetFromStringTest")]
        public void GetFromStringTest()
        {
            string[] dump = new string[] 
            {
                "a", "A",
                "b", "B",
                "x", "X",
                "m", ""
            };
            ObjectType[] results = new ObjectType[]
            {
                ObjectType.StartPoint, ObjectType.StartPoint,
                ObjectType.EndPoint, ObjectType.EndPoint,
                ObjectType.Block, ObjectType.Block,
                ObjectType.None, ObjectType.None
            };

            ObjectType type = ObjectType.None;
            for (int i = 0; i < dump.Length; i++)
            {
                type = ObjectTypeStrings.GetFromString(dump[i]);
                Assert.AreEqual(results[i], type);
            }
        }

        [TestMethod()]
        [TestCategory("ObjectTypeStrings GetFromStringTest")]
        public void GetFromStringTest_UnknownString_returns_None()
        {
            string randomUnknownKey = "";
            Random rand = new Random();
            int stopCounter = 0, maxCycles = 1000;
            while (ObjectTypeStrings.Dictionary.ContainsKey(randomUnknownKey) && stopCounter < maxCycles)
            {
                randomUnknownKey = rand.Next().ToString();
                stopCounter++;
            }
            if (stopCounter >= maxCycles)
            {
                Assert.Fail("Can't find unknown key");
            }
            var resultType = ObjectTypeStrings.GetFromString(randomUnknownKey);
            Assert.AreEqual(ObjectType.None, resultType);
        }
    }
}