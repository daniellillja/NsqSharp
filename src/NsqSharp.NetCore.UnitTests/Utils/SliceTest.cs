using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NsqSharp.NetCore.UnitTestMethods;
using NsqSharp.Utils;

namespace NsqSharp.Tests.Utils
{
    [TestClass]
    public class SliceTest
    {
        [TestMethod]
        public void TestStringEquality()
        {
            var x = new Slice<char>("abcdef");
            var y = "abcdef";

            Assert.IsTrue(x == y);
        }

        [TestMethod]
        public void TestStringEqualityAfterSlice()
        {
            var x = new Slice<char>("abcdef");
            x = x.Slc(0, 3);
            var y = "abc";

            Assert.IsTrue(x == y);
        }

        [TestMethod]
        public void TestStringEqualityAfterMidSlice()
        {
            var x = new Slice<char>("abcdef");
            x = x.Slc(1, 4);
            var y = "bcd";

            Assert.IsTrue(x == y);
        }

        [TestMethod]
        public void TestStringTypeEqualityMismatch()
        {
            var x = new Slice<int>(new[] { 1, 2, 3 });
            var y = "abc";

            Assert.IsFalse(x == y);
        }

        [TestMethod]
        public void TestNullStringEquality()
        {
            Slice<char> x = null;
            string y = null;

            Assert.IsTrue(x == y);
        }

        [TestMethod]
        public void TestRightNullStringInequality()
        {
            var x = new Slice<char>("abc");
            string y = null;

            Assert.IsTrue(x != y);
        }

        [TestMethod]
        public void TestLeftNullSliceInequality()
        {
            Slice<char> x = null;
            var y = "abc";

            Assert.IsTrue(x != y);
        }

        [TestMethod]
        public void TestToStringOnSliceChar()
        {
            var x = new Slice<char>("hello world");
            var actual = x.Slc(2, 8).ToString();

            Assert.AreEqual("llo wo", actual);
        }

        [TestMethod]
        public void TestToStringOnSliceInt()
        {
            var x = new Slice<int>(new[] { 1, 2, 3, 4 });
            var actual = x.ToString();

            Assert.AreEqual("[ 1 2 3 4 ]", actual);
        }

        [TestMethod]
        public void TestSlcHasNoSideEffects()
        {
            var x = new Slice<char>("hello world");
            x.Slc(2, 8);

            Assert.AreEqual("hello world".Length, x.Len());
            Assert.AreEqual("hello world", x.ToString());
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSliceStartLessThan0()
        {
            var x = new Slice<char>("test");

            x.Slc(-1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSliceStartGreaterThanLen()
        {
            var x = new Slice<char>("test");

            x.Slc(5);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSliceStartGreaterThanEnd()
        {
            var x = new Slice<char>("test");

            x.Slc(2, 1);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSliceEndGreaterThanLen()
        {
            var x = new Slice<char>("test");

            x.Slc(2, 5);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSliceEndGreaterThanLenAfterReslice()
        {
            var x = new Slice<char>("test");

            x = x.Slc(0, 2);

            x.Slc(0, 3);
        }

        [TestMethod]
        public void TestSliceAtZero()
        {
            var x = new Slice<char>("test!");
            x = x.Slc(0, 1);

            Assert.AreEqual("t", x.ToString());
        }

        [TestMethod]
        public void TestSliceAtEnd()
        {
            var x = new Slice<char>("test!");
            x = x.Slc(x.Len() - 1);

            Assert.AreEqual("!", x.ToString());
        }

        [TestMethod]
        public void TestEmptySliceAtZero()
        {
            var x = new Slice<char>("test!");
            x = x.Slc(0, 0);

            Assert.AreEqual("", x.ToString());
        }

        [TestMethod]
        public void TestEmptySliceAtEnd()
        {
            var x = new Slice<char>("test!");
            x = x.Slc(x.Len());

            Assert.AreEqual("", x.ToString());
        }

        [TestMethod]
        public void TestEmptySliceAtMid()
        {
            var x = new Slice<int>(new[] { 1, 2, 3, 4 });
            x = x.Slc(1, 1);

            AssertHelper.IsEmpty(x.ToArray());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestStringConstructorThrowsOnNull()
        {
            new Slice<char>((string)null);
        }

        [TestMethod]
        public void TestStringConstructorThrowsOnTypeMistmatch()
        {
            AssertHelper.Throws<Exception>(() => new Slice<int>("nope"));
        }

        [TestMethod]
        public void TestArrayConstructorThrowsOnNull()
        {
            AssertHelper.Throws<ArgumentNullException>(() => new Slice<int>((int[])null));
        }

        [TestMethod]
        public void TestIndexOfSlice()
        {
            var s = new Slice<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            int i;
            for (i = 0; i < s.Len(); i++)
                Assert.AreEqual(i + 1, s[i]);
            AssertHelper.Throws<IndexOutOfRangeException>(() =>
            {
                var a = s[i];
            });
        }

        [TestMethod, ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestIndexOfSliceOfSlice()
        {
            var s = new Slice<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            s = s.Slc(2, 5);

            int i;
            for (i = 0; i < s.Len(); i++)
                Assert.AreEqual(i + 3, s[i]);
            var a = s[i];
        }

        [TestMethod]
        public void TestIndexOfSliceOfSliceOfSlice()
        {
            var s = new Slice<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            // 4, 5, 6, 7, 8, 9
            s = s.Slc(3, 9);

            int i;
            for (i = 0; i < s.Len(); i++)
                Assert.AreEqual(i + 4, s[i]);

            AssertHelper.Throws<IndexOutOfRangeException>(() =>
            {
                var a = s[i];
            }); ;

            // 5, 6, 7, 8
            s = s.Slc(1, 5);

            for (i = 0; i < s.Len(); i++)
                Assert.AreEqual(i + 5, s[i]);
            AssertHelper.Throws<IndexOutOfRangeException>(() =>
            {
                var a = s[i];
            });
        }

        [TestMethod]
        public void TestGetHashCodeIsZeroForEmptySlice()
        {
            var s = new Slice<int>(new int[0]);
            Assert.AreEqual(0, s.GetHashCode());

            s = new Slice<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            s = s.Slc(5, 5);
            Assert.AreEqual(0, s.Len());
            Assert.AreEqual(0, s.GetHashCode());
        }

        [TestMethod]
        public void TestGetHashCodeIsCalculating()
        {
            var s = new Slice<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            var hash1 = s.GetHashCode();

            s = new Slice<int>(new[] { 1, 2, 3, 4, 5 });
            var hash2 = s.GetHashCode();

            Assert.AreNotEqual(0, hash1);
            Assert.AreNotEqual(0, hash2);
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}