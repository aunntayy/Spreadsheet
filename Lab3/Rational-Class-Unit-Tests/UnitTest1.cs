using Microsoft.VisualStudio.TestTools.UnitTesting;
using RationalNumbers;
namespace Rational_Class_Unit_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RationalConstructorTest()
        {
            Rational z = new Rational();
            Assert.AreEqual("0", z.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RationalConstructorTest2()
        {
            Rational r = new Rational(5, 0);
        }

        [TestMethod]
        public void Test3()
        {
            Rational r1 = new Rational(1, 3);
            Rational r2 = new Rational(2, 3);
            Assert.AreEqual(r1, r2);
            Assert.IsTrue(r1.Equals(r2));
            Assert.IsTrue(r2.Equals(r1));
            // check that the equality  (use AreNotEqual)
            // Check the ==    (use IsFalse)
            // Check the !=    (use IsTrue)
        }


        [TestMethod]
        public void Test4()
        {
            Rational r1 = new Rational(1, 3);
            Rational r2 = new Rational(2, 3);
            Assert.AreEqual("1", (r1 + r2).ToString());
            // check that the result of adding these when printed as a string
            //     results in the value "1"
        }

        ///[TestMethod]
        /// public void TestStackOverflow()
        ///{
        ///Rational.InfiniteRecursion();
        ///}

        [TestMethod]
        public void DenomNumConstructorTest()
        {
            Rational test_rational = new Rational(1, 2);
            Assert.AreEqual("1/2", $"{test_rational}");
        }
        [TestMethod]
        public void NegaiveNumConstructorTest()
        {
            Rational test_rational = new Rational(1, -2);
            Assert.AreEqual("-1/2", $"{test_rational}");
        }

    }
}