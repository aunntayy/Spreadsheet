using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    /// <summary>
    /// Author:    Phuc Hoang
    /// Partner:   -None-
    /// Date:      3-Feb-2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and Phuc Hoang - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, Phuc Hoang, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    [This is a test for Formula]
    ///    
    /// </summary>

    /// <summary>
    ///This is a test class for Formula Test
    ///</summary>
    [TestClass]
    public class FormulaTests
    {
        //Parsing Rule Test - formula construct
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void oneTokenTest()
        {
            Formula f = new Formula("");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleRightParenthesisTest()
        {
            Formula f = new Formula("(3+1))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleStartingTokenTest()
        {
            Formula f = new Formula("+1-2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleEndingTokenTest()
        {
            Formula f = new Formula("1-2+");
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
            Formula f = new Formula("1++2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleExtrarFollowingTest()
        {
            Formula f = new Formula("(1+2)2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleBalanceParenthesisTest()
        {
            Formula f = new Formula("(3+1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void complexBalanceParenthesisTest()
        {
            Formula f = new Formula("(1+2)-(3+1))");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void simpleTokenRuleTest()
        {
            Formula f = new Formula("2+@");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NormalizedVarTest()
        {
            //this line is to throw the situation if the normalize func changed the variable into something that is not valid
            Formula f = new Formula("var1 + 3", s => s.Replace("var1", "1a"), s => false);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void invalidNormalizedVarTest()
        {
            Formula f = new Formula("var + 3", s => s.ToUpper(), s => false);
        }

        //End of parsing Rule test

        // Divine by zero test
        [TestMethod]
        public void simpleDivineByZeroTest()
        {
            Formula f = new Formula("1/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        public void divindeByZeroWithParenthesisTest()
        {
            Formula f = new Formula("(1+1)/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod]
        public void complexDivindeByZeroTest()
        {
            Formula f = new Formula("(1+A1)/(A1-12)");
            Assert.IsInstanceOfType(f.Evaluate(s => 12), typeof(FormulaError));
        }

        [TestMethod]
        public void varDivindeByZeroTest()
        {
            Formula f = new Formula("1/X1");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }
        //End of divine by zero test

        //Evaluate test
        [TestMethod]
        public void simpleMathTest()
        {
            Formula f1 = new Formula("1+2");
            Formula f2 = new Formula("2-2");
            Formula f3 = new Formula("2/2");
            Formula f4 = new Formula("2*2");
            Assert.AreEqual(3.0, f1.Evaluate(s => 0));
            Assert.AreEqual(0.0, f2.Evaluate(s => 0));
            Assert.AreEqual(1.0, f3.Evaluate(s => 0));
            Assert.AreEqual(4.0, f4.Evaluate(s => 0));
        }

        [TestMethod]
        public void manyVarTest()
        {
            Formula f = new Formula("yy1y*3-8/2+4*(8-92)/14*x7");
            Assert.AreEqual(-16.0, f.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod]
        public void complexMathTest()
        {
            Formula f1 = new Formula("2*6+3");
            Formula f2 = new Formula("(2+6)*3");
            Formula f3 = new Formula("(1*1)-2/2");
            Formula f4 = new Formula("2+3*5+(3+4*8)*5+2");
            Formula f5 = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Formula f6 = new Formula("2+3*(3+5)");
            Formula f7 = new Formula("a4 - a4 * a4 / a4"); 
            Formula f8 = new Formula("2+6*3");
            Formula f9 = new Formula("2*(3+5)");
            Formula f10 = new Formula("2+(3+5)");
            Formula f11 = new Formula("2+(3+5*9)");
            Formula f12 = new Formula("(1*1)-2/2");
            Assert.AreEqual(15.0, f1.Evaluate(s => 0));
            Assert.AreEqual(24.0, f2.Evaluate(s => 0));
            Assert.AreEqual(0.0, f3.Evaluate(s => 0));
            Assert.AreEqual(194.0, f4.Evaluate(s => 0));
            Assert.AreEqual(12.0, f5.Evaluate(s => 2));
            Assert.AreEqual(26.0, f6.Evaluate(s => 0));
            Assert.AreEqual(0.0, f7.Evaluate(s => 3));
            Assert.AreEqual(20.0, f8.Evaluate(s => 0));
            Assert.AreEqual(16.0, f9.Evaluate(s => 0));
            Assert.AreEqual(10.0, f10.Evaluate(s => 0));
            Assert.AreEqual(50.0, f11.Evaluate(s => 0));
            Assert.AreEqual(0.0, f12.Evaluate(s => 0));
        }


        [TestMethod]
        public void testComplexNestedParensRight()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void undefindedValueTest()
        {
            Formula f = new Formula("A1+2");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Variable is not definded"); }), typeof(FormulaError));
        }
        //End of Evaluate test

        //Get variable test
        [TestMethod]
        public void simpleGetVariableTest()
        {
            Formula f = new Formula("X1+Y1");
            IEnumerable<string> variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("X1"));
            Assert.IsTrue(variables.Contains("Y1"));
            Assert.AreEqual(2, variables.Count());
        }

        [TestMethod]
        public void getVaribleWithNomarlizerTest()
        {
            Formula f = new Formula("B1 + a1", x => x.ToUpper(), x => true);
            IEnumerable<string> variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("A1"));
            string formula = f.ToString();
            Assert.AreEqual("B1+A1", formula);
        }
        //End of get variable test

        //To string test
        [TestMethod]
        public void simpleToStringTest()
        {
            Formula f = new Formula("1+2");
            object result = f.Evaluate(s => 0);
            Assert.AreEqual(3.0, result);
        }

        //Equal test 
        [TestMethod]
        public void equalTest1()
        {
            Formula f1 = new Formula("A1+A2");
            Formula f2 = new Formula("A2+A2");
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        public void equalTest2()
        {
            Formula f1 = new Formula("1.0000000000000000+X2");
            Formula f2 = new Formula("1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod]
        public void equalTest3()
        {
            Formula f1 = new Formula("123 + 321");
            object f2 = new String("123 +321");
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        public void equalTest4()
        {
            Formula f1 = new Formula("123");
            object f2 = null;
            Assert.IsFalse(f1.Equals(f2));
        }

        [TestMethod]
        public void equalTest5()
        {
            Formula f1 = new Formula("1.029102902898 + 2");
            Formula f2 = new Formula("1.0929 + 2");
            Assert.IsFalse(f1.Equals(f2));
        }
        //End of equal test

        //Boolean test
        [TestMethod]
        public void trueEqualBoolTest()
        {
            Formula f1 = new Formula("a3+2");
            Formula f2 = new Formula("a3+2");
            Assert.IsTrue(f1 == f2);
        }
        
        [TestMethod]
        public void falseEqualBoolTest() 
        {
            Formula f1 = new Formula("2*3+A1");
            Formula f2 = new Formula("12*3+A1");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod]
        public void trueUnequalBoolTest()
        {
            Formula f1 = new Formula("2*x6");
            Formula f2 = new Formula("2*x4");
            Assert.IsTrue(f1 != f2);
        }

        [TestMethod]
        public void falseUnequalBoolTest()
        {
            Formula f1 = new Formula("2*3");
            Formula f2 = new Formula("2*3");
            Assert.IsFalse(f1 != f2);
        }

        //Get hash code test
        [TestMethod]
        public void getHashTest()
        {
            Formula f1 = new Formula("A1 + B2");
            Formula f2 = new Formula("A1+B2");
            int form1 = f1.GetHashCode();
            int form2 = f2.GetHashCode();
            Assert.AreEqual(form1, form2);
        }
    }
}