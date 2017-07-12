using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils.Extensions;

namespace NsqSharp.Tests.Utils.Extensions
{
    [TestClass]
    public class RNGCryptoServiceProviderExtensionsTest
    {
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        [TestMethod]
        public void Float64()
        {
            for (int i = 0; i < 10000; i++)
            {
                double value = _rng.Float64();
                Assert.IsTrue(value >= 0);
                Assert.IsTrue(value < 1);
            }
        }

        [TestMethod]
        public void IntnRange()
        {
            AssertHelper.Throws<ArgumentOutOfRangeException>(() => _rng.Intn(0));
            AssertHelper.Throws<ArgumentOutOfRangeException>(() => _rng.Intn(-1));
        }

        [TestCleanup]
        public void TearDown()
        {
            _rng.Dispose();
        }
    }
}
