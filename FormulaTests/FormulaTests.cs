using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        //Parsing Rule Test
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
        //End of parsing Rule test

        // Divine by zero test
        [TestMethod]
        public void SimpleDivineByZeroTest()
        {
            Formula f = new Formula("1/0");
            Assert.IsInstanceOfType(f.Evaluate( s => 0), typeof(FormulaError));
        }

        [TestMethod]
        public void DivindeByZeroWithParenthesisTest()
        {
            Formula f = new Formula("(1+1)/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        public void ComplexDivindeByZeroTest() 
        {
            Formula f = new Formula("(1 + X1) / (X1 - 12)");
            Assert.IsInstanceOfType(f.Evaluate(s => 12), typeof(FormulaError));
        }
        //End of divine by zero test

        //Evaluate test

        public void simpleMathTest()
        {
            Formula f1 = new Formula("1+2");
            Formula f2 = new Formula("2-2");
            Formula f3 = new Formula("2/2");
            Formula f4 = new Formula("2*2");
            Assert.AreEqual(3, f1.Evaluate(s => 0));
            Assert.AreEqual(0, f2.Evaluate(s => 0));
            Assert.AreEqual(1, f3.Evaluate(s => 0));
            Assert.AreEqual(4, f4.Evaluate(s => 0));
        }

        //End of Evaluate test
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

        [TestMethod()]
        public void GetVaribleWithNomarlizerTest()
        {
            Formula f = new Formula("B1 + a1", x => x.ToUpper(), x => true);
            IEnumerable<string> variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("A1"));
            string formula = f.ToString();
            Assert.AreEqual("B1+A1", formula);
        }

        [TestMethod]
        public void EqualTest1()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X2+X2");
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        public void EqualTest2()
        {
            Formula f1 = new Formula("1.0000000000000000+X2");
            Formula f2 = new Formula("1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }
        [TestMethod]
        public void testBoolFormula()
        {
            Formula f1 = new Formula("2*x3", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2*x3", s => s.ToUpper(), s => true);
            bool areEqual = f1 == f2;
            Assert.IsTrue(areEqual);

            f1 = new Formula("2*x6", s => s.ToUpper(), s => true);
            f2 = new Formula("2*x4", s => s.ToUpper(), s => true);
            bool areNotEqual = f1 != f2;
            Assert.IsTrue(areNotEqual);
        }
    }
}