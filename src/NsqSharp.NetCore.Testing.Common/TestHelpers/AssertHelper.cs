using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NsqSharp.NetCore.UnitTestMethods
{
    public static class AssertHelper
    {
        public static T Throws<T>(Action a, string reason = null)
            where T : class
        {
            try
            {
                a.Invoke();
            }
            catch (Exception e)
            {
                if (reason == null)
                {
                    Assert.IsInstanceOfType(e, typeof(T));
                }

                Assert.IsInstanceOfType(e, typeof(T), reason);
                return e as T;
            }

            return default(T);
        }

        public static void IsEmpty(int[] toArray)
        {
            Assert.IsTrue(toArray.Length.Equals(0));
        }

        public static void GreaterOrEqual(TimeSpan first, TimeSpan second, string reason)
        {
            Assert.IsTrue(first >= second, reason);
        }

        public static void Less(TimeSpan first, TimeSpan second, string reason = null)
        {
            if (reason != null)
            {
                Assert.IsTrue(first < second, reason);
            }

            Assert.IsTrue(first < second);
        }

        public static void Throws(Type expectedException, Action a)
        {
            try
            {
                a.Invoke();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, expectedException);
            }
        }

        public static void LessOrEqual(TimeSpan first, TimeSpan second, string reason)
        {
            Assert.IsTrue(first <= second, reason);
        }

        public static void AreEqual<T>(T[] array, List<T> list, string reason = null)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array[i], list[i], $"element {i} mismatch", reason);
            }
        }
    }
}
