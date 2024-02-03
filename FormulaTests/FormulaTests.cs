using SpreadsheetUtilities;
using System.Text.RegularExpressions;

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
        public void validVarTest()
        {
            Formula f = new Formula("12b");
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
            Formula f = new Formula("X1+Y1");
            IEnumerable<string> variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("X1"));
            Assert.IsTrue(variables.Contains("Y1"));
            Assert.AreEqual(2, variables.Count());
        }

        [TestMethod]
        public void SimpleToStringTest() 
        {
            Formula f = new Formula("1+2");
            object result = f.Evaluate(s => 0);
            Assert.AreEqual(3.0, result);
        }

        [TestMethod]
        public void ManyVarTest()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-92)/14*x7");
            Assert.AreEqual(-16.0, f.Evaluate(s => (s == "x7") ? 1 : 4));
        }
    }
}