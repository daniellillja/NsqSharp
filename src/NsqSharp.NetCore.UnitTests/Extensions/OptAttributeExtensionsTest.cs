using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils.Attributes;
using NsqSharp.Utils.Extensions;

namespace NsqSharp.Tests.Utils.Extensions
{
    [TestClass]
    public class OptAttributeExtensionsTest
    {
        [TestMethod]
        public void Coerce()
        {
            var opt = new OptAttribute("testName");
            var coercedVal = opt.Coerce("12ms", typeof(TimeSpan));
            Assert.AreEqual(TimeSpan.FromMilliseconds(12), coercedVal);
        }

        [TestMethod]
        public void InvalidCoerceThrows()
        {
            var opt = new OptAttribute("testName");

            var ex = AssertHelper.Throws<Exception>(() => opt.Coerce("12xs", typeof(TimeSpan)));
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Message);
            Assert.IsTrue(ex.Message.Contains(opt.Name));
        }
    }
}
