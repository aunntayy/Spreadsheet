// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!
// Version 1.1 (9/22/13 11:45 a.m.)
// Change log:
// (Version 1.1) Repaired mistake in GetTokens
// (Version 1.1) Changed specification of second constructor to
// clarify description of how validation works
// (Daniel Kopta)
// Version 1.2 (9/10/17)
// Change log:
// (Version 1.2) Changed the definition of equality with regards
// to numeric tokens
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq.Expressions;
using System.Data.SqlTypes;
using System.Data;
using System.Diagnostics;
namespace SpreadsheetUtilities
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
    ///    [This library is a refactored of FormualEvaluator class. Formula class is a more generalized version of FormulaEvaluator work. 
    ///    This formula class will be utilized in the upcoming spreadsheet assignment.]
    ///    
    /// </summary>

    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules. The allowed symbols are non-negative numbers written using double-precision
    /// floating-point syntax (without unary preceeding '-' or '+');
    /// variables that consist of a letter or underscore followed by
    /// zero or more letters, underscores, or digits; parentheses; and the four operator
    /// symbols +, -, *, and /.
    ///
    /// Spaces are significant only insofar that they delimit tokens. For example,"xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
    /// and "x 23" consists of a variable "x" and a number "23".
    ///
    /// Associated with every formula are two delegates: a normalizer and a validator.The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.) Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> tokens;
        private List<string> normVar;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment. If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {

        }
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment. If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        ///
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.
        ///
        /// If the formula contains a variable v such that normalize(v) is not a legal variable,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        ///
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit. Then:
        ///
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {

            tokens = GetTokens(formula).ToList();



            //Specific token Rule
            string specificRule = @"^(\(|\)|\+|-|\*|\/|[a-zA-Z_][a-zA-Z0-9_]*|[-+]?\d*\.?\d+([eE][-+]?\d+)?)$";
            if (!tokens.TrueForAll(token => Regex.IsMatch(token, specificRule)))
            {
                throw new FormulaFormatException("Invalid input");
            }

            // One Token rule
            if (tokens.Count == 0)
            {
                throw new FormulaFormatException("Need to have at least 1 valid token");
            }


            //Keep track of "(" and ")"
            int open = 0;
            int close = 0;
            foreach (string token in tokens)
            {
                if (token == ("(")) { open++; }
                if (token == (")")) { close++; }
            }
            //Right Parenthesis Rule
            if (close > open) { throw new FormulaFormatException("There cannot be more closing parenthesis than open parnethesis"); }
            //Balanced Parenthesis Rule
            if (open != close) { throw new FormulaFormatException("mismatch parenthesis"); }

            //Var to track of first and last token
            string firstToken = tokens.First<string>();
            string lastToken = tokens.Last<string>();
            //Regex to recognize operator 
            string operatorPattern = @"^[*/+\-]$";
            //Starting Token Rule
            if (Regex.IsMatch(firstToken, operatorPattern))
            {
                throw new FormulaFormatException("Starting of the input need to be a number, a variable, or an opening parenthesis.");
            }
            //Ending Token Rule
            if (Regex.IsMatch(lastToken, operatorPattern))
            {
                throw new FormulaFormatException("Ending of the input need to be a number, a variable, or an opening parenthesis.");
            }

            //Regex to recognize "(, *, /, +, -"
            string followRule = @"^[\(+*/\-]$";
            //Regex to recognize a number, a variable, or a closing parenthesis 
            string extraFollowRule = @"^[-+]?\d*\.?\d+([eE][-+]?\d+)?$|[a-zA-Z_][a-zA-Z0-9_]*|\)$";
            //Regex to recognize a number, a variable, or an opening parenthesis.
            string validTokenPattern = @"^[-+]?\d*\.?\d+([eE][-+]?\d+)?$|[a-zA-Z_][a-zA-Z0-9_]*|\($";
            //Regex to recognize "(, *, /, +, -"
            string extraValidTokenPattern = @"^[\)+*/\-]$";
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                string currentToken = tokens[i];
                string nextToken = tokens[i + 1];
                //Parenthesis, operator following rule
                if (Regex.IsMatch(currentToken, followRule))
                {
                    if (!Regex.IsMatch(nextToken, validTokenPattern))
                    {
                        throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                    }
                }
                //Extra following rule
                if (Regex.IsMatch(currentToken, extraFollowRule))
                {
                    if (!Regex.IsMatch(nextToken, extraValidTokenPattern))
                    {
                        throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                    }
                }
            }

            normVar = new List<string>();
            for (int i = 0; i < tokens.Count; i++)
            {
                string var = tokens[i];

                if (isVar(var))
                {
                    string normalizedVar = normalize(var);
                    /// If the formula contains a variable v such that normalize(v) is not a legal variable,
                    /// throws a FormulaFormatException with an explanatory message.
                    if (!(isVar(normalizedVar)))
                    {
                        throw new FormulaFormatException("Not a valid variable format after normalized");
                    }
                    /// If the formula contains a variable v such that isValid(normalize(v)) is false,
                    /// throws a FormulaFormatException with an explanatory message.
                    if (!(isValid(normalizedVar)))
                    {
                        throw new FormulaFormatException("Not a valid variable after normalized");
                    }

                    tokens[i] = normalizedVar;
                    normVar.Add(tokens[i]);
                }
            }

        }
        //Check for valid variable
        private bool isVar(string token)
        {
            return Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }


        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables. When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to
        /// the constructor.)
        ///
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters
        /// in a string to upper case:
        ///
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        ///
        /// Given a variable symbol as its parameter, lookup returns the variable's value
        /// (if it has one) or throws an ArgumentException (otherwise).
        ///
        /// If no undefined variables or divisions by zero are encountered when evaluating
        /// this Formula, the value is returned. Otherwise, a FormulaError is returned.
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<double> valstack = new();
            Stack opstack = new();
            string[] substrings = tokens.Cast<string>().ToArray<string>();

            // Check each substring 
            foreach (string substring in substrings)
            {
                //if t is an integer
                if (double.TryParse(substring, out double n))
                {
                    //For mutiply and divine
                    if (valstack.Count > 0 && opstack.Count > 0 && (opstack.Peek().ToString() == "*" || opstack.Peek().ToString() == "/"))
                    {
                        double num1 = (double)valstack.Pop();
                        double num2 = n;
                        string op = (string)opstack.Pop();
                        if (num2 == 0 && op == "/")
                        {
                            return new FormulaError("Cannot divine by zero");
                        }
                        valstack.Push(math(num1, op, num2));
                    }
                    //push t onto value stack
                    else { valstack.Push(n); }
                }
                //if t is a variable
                if (Regex.IsMatch(substring, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                {
                    double variableValue = 0;
                    //Use try catch block to return FormulaError
                    try
                    {
                        // Handle variables using the variableEvaluator delegate
                        variableValue = lookup(substring);
                    }
                    catch (ArgumentException error)
                    {
                        return new FormulaError("Variable is not definded");
                    }
                    //For mutiply and divine
                    if (valstack.Count > 0 && opstack.Count > 0 && (opstack.Peek().ToString() == "*" || opstack.Peek().ToString() == "/"))
                    {
                        double num1 = (double)valstack.Pop();
                        double num2 = variableValue;
                        string op = (string)opstack.Pop();
                        if (num2 == 0 && op == "/")
                        {
                            return new FormulaError("Cannot divine by zero");
                        }
                        valstack.Push(math(num1, op, num2));
                    }
                    //push t onto value stack
                    else { valstack.Push(variableValue); }
                }
                //if t = "+" or "-"
                if (substring == "+" || substring == "-")
                {
                    if (valstack.Count == 2 && opstack.Count > 0 && (opstack.Peek().Equals("+") || opstack.Peek().Equals("-")))
                    {
                        if (opstack.Peek().Equals("+"))
                        {
                            double num1 = (double)valstack.Pop();
                            double num2 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, num2));
                        }
                        else if (opstack.Peek().Equals("-"))
                        {
                            double num1 = (double)valstack.Pop();
                            double num2 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(num2 - num1);
                        }
                    }
                    opstack.Push(substring);
                }
                //if t = "*", "/", "("
                if (substring == "*" || substring == "/" || substring == "(")
                {
                    opstack.Push(substring);
                }

                // if t = ")"
                if (substring == ")")
                {


                    // Evaluate expressions inside parentheses
                    while (opstack.Count > 0 && opstack.Peek().ToString() != "(")
                    {
                        double num1 = valstack.Pop();
                        double num2 = valstack.Pop();
                        string op = opstack.Pop().ToString();
                        valstack.Push(math(num2, op, num1));
                    }

                    opstack.Pop();


                    // Check if there are additional operators (* or /) after parentheses
                    if (opstack.Count > 0 && (opstack.Peek().ToString().Trim() == "*" || opstack.Peek().ToString().Trim() == "/"))
                    {
                        double num1 = valstack.Pop();
                        double num2 = valstack.Pop();
                        string op = opstack.Pop().ToString();
                        if (num1 == 0 && op == "/")
                        {
                            return new FormulaError("Cannot divine by zero");
                        }
                        valstack.Push(math(num2, op, num1));
                    }
                }

            }

            // Process any remaining operators
            while (opstack.Count > 0 && valstack.Count >= 2)
            {
                double num1 = (double)valstack.Pop();
                double num2 = (double)valstack.Pop();
                string op = (string)opstack.Pop();
                if (num1 == 0 && op == "/")
                {
                    return new FormulaError("Cannot divine by zero");
                }
                valstack.Push(math(num2, op, num1));
            }
            double finalVal = (double)valstack.Pop();
            return finalVal;
        }
        /// <summary>
        /// Method to do the math work depend on the operation
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="op"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        private static double math(double num1, string op, double num2)
        {
            double value = 0;
            if (op == "+") { value = num1 + num2; }
            else if (op == "-") { value = num1 - num2; }
            else if (op == "*") { value = num1 * num2; }
            else if (op == "/") { value = num1 / num2; }
            return value;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this
        /// formula. No normalization may appear more than once in the enumeration, even
        /// if it appears more than once in this Formula.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X","Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> variables = normVar.Where(token => isVar(token)).ToList();
            return variables;
        }


        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f). All of the
        /// variables in the string should be normalized.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            StringBuilder strings = new StringBuilder();
            foreach (string token in tokens)
            {
                if (token != " ")
                {
                    strings.Append(token);
                }
            }
            return strings.ToString();
        }
        /// <summary>
        /// <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false. Otherwise, reports
        /// whether or not this Formula and obj are equal.
        ///
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order. To determine token equality, all tokens are compared as strings
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized"
        /// by C#'s standard conversion from string to double, then back to string. This
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal,as
        /// defined by the provided normalizer.
        ///
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1 + Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            //If obj is null or obj is not a Formula, returns false.
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            //Set obj as formula type
            Formula formula = obj as Formula;

            for (int i = 0; i < this.tokens.Count; i++)
            {
                //Hold current token of each formula
                string formula1 = this.tokens[i];
                string formula2 = formula.tokens[i];

                //Hold number after parsing
                double formNumber1;
                double formNumber2;

                //Convert string into double then check
                if ((Double.TryParse(formula1, out formNumber1)) && (Double.TryParse(formula2, out formNumber2)))
                {
                    //Check if the 2 double are equal
                    if (formNumber1 != formNumber2)
                        return false;
                }
                else
                {
                    //Check if the 2 string var are equal
                    if (!(formula1.Equals(formula2)))
                        return false;
                }
            }
            return true;
        }
        /// <summary>
        /// <change> We are now using Non-Nullable objects. Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        ///
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null) { return true; }
            if (f1 is null || f2 is null) { return false; }
            //Use the notion of equality from the Equals method.
            return f1.Equals(f2);
        }
        /// <summary>
        /// <change> We are now using Non-Nullable objects. Thus neither f1 nor f2 can be null!</change>
        /// <change> Note: != should almost always be not ==, if you get my meaning </change>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (!f1.Equals(f2)) return true;
            return false;
        }
        /// <summary>
        /// Returns a hash code for this Formula. If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode(). Ideally, the probability that two
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int getHash = this.ToString().GetHashCode();
            return getHash;
        }
        /// <summary>
        /// Given an expression, enumerates the tokens that compose it. Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns. There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";
            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) |({5})", lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);
            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
        : base(message)
        {
        }
    }
    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
        : this()
        {
            Reason = reason;
        }
        /// <summary>
        /// The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}

// <change>
// If you are using Extension methods to deal with common stack operations (e.g., checking for
// an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
// To fix this, you have to use a little special syntax like the following:
//
// public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
// Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
// as long as it doesn't allow nulls!
// </change>