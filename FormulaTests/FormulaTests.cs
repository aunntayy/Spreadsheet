using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OneTokenTest()
        {
            Formula f = new Formula("");
        }
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleRightParenthesisTest()
        {
            Formula f = new Formula("(3+1))");
        }  
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleStartingTokenTest()
        {
            Formula f = new Formula("+1-2");
        } 
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleEndingTokenTest()
        {
            Formula f = new Formula("1-2+");
        }  
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleParenthesisOperatorFollowingTest()
        {
            Formula f = new Formula("(1+2)*)");
        } 
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleExtrarFollowingTest()
        {
            Formula f = new Formula("(1+2)2");
        }
        
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleBalanceParenthesisTest() 
        {
            Formula f = new Formula("(3+1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ComplexBalanceParenthesisTest()
        {
            Formula f = new Formula("(1+2)-(3+1))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SimpleTokenRuleTest()
        {
            Formula f = new Formula("2+@");
        }

        [TestMethod]
        public void SimpleGetVariableTest() 
        {
            Formula f = new Formula("X+Y");
            IEnumerable<string> variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("X"));
            Assert.IsTrue(variables.Contains("Y"));
            Assert.AreEqual(2, variables.Count());
        }

        [TestMethod]
        public void SimpleToStringTest() 
        {
            Formula f = new Formula("1 + 2");
            string result = f.ToString();
            Assert.AreEqual("1+2", result);
        }

        [TestMethod()]
        public void Test18()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }
    }
}