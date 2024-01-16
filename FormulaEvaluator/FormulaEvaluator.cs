using System.Collections;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// Author:    Phuc Hoang
    /// Partner:   -None-
    /// Date:      1-Jan-2024
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
    ///    [This library evaluates arithmetic expressions using standard infix notation.]
    ///    
    /// </summary>

    public static class Evaluator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable_name"></param>
        /// <returns></returns>
        public delegate int Lookup(String variable_name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            Stack<int> valstack = new();
            Stack opstack = new();


            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Check each substring 
            foreach (string substring in substrings) {
                //if t is an integer
                if (int.TryParse(substring, out int n))
                {
                    //For mutiply and divine
                    if (valstack.Count > 0 && opstack.Count > 0 && (opstack.Peek().ToString() == "*" || opstack.Peek().ToString() == "/"))
                    {
                        int num1 = (int)valstack.Pop();
                        int num2 = n;
                        string op = (string)opstack.Pop();
                        valstack.Push(math(num1, op, num2));
                    }
                    //push t onto value stack
                    else { valstack.Push(n); }
                }
                //if t is a variable
                else if (Regex.IsMatch(substring, @"[a-zA-Z_]\w*"))
                {
                    // Handle variables using the variableEvaluator delegate
                    int variableValue = variableEvaluator(substring);
                    //For mutiply and divine
                    if (valstack.Count > 0 && opstack.Count > 0 && (opstack.Peek().ToString() == "*" || opstack.Peek().ToString() == "/"))
                    {
                        int num1 = (int)valstack.Pop();
                        int num2 = variableValue;
                        string op = (string)opstack.Pop();
                        valstack.Push(math(num1, op, num2));
                    }
                    //push t onto value stack
                    else { valstack.Push(variableValue); }
                }
                //if t = "+" or "-"
                else if (substring == "+" || substring == "-")
                {
                    if (valstack.Count >=2 && opstack.Count > 0 && (opstack.Peek() == "+" || opstack.Peek() == "-"))
                    {
                        int num1 = (int)valstack.Pop();
                        int num2 = (int)valstack.Pop();
                        string op = (string)opstack.Pop();
                        valstack.Push(math(num2, op, num1));  
                    }
                    opstack.Push(substring);
                    
                }
                
                //if t = "*", "/", "("
                else if (substring == "*" || substring == "/" || substring == "("){
                    opstack.Push(substring);
                }
                
                // if t = ")"
                else if (substring == ")") {
                    if (valstack.Count >= 1)
                    {
                        //if top is + or "-"
                        if (valstack.Count >=1 && opstack.Count > 0 && (opstack.Peek().ToString().Trim() == "+" || opstack.Peek().ToString().Trim() == "-"))
                        {
                            int num1 = (int)valstack.Pop();
                            int num2 = (int)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num2, op, num1));
                        }
                        if (opstack.Count == 0 && opstack.Peek().ToString() != "(")
                        {
                            throw new Exception("Mismatched parentheses");
                        }
                        else
                        {
                            opstack.Pop(); // Pop the "("
                        }
                        if (valstack.Count >= 2)
                        {
                            // If * or / is at the top
                            if (valstack.Count > 2 && opstack.Count > 0 && (opstack.Peek().ToString().Trim() == "*" || opstack.Peek().ToString().Trim() == "/"))
                            {
                                int num1 = (int)valstack.Pop();
                                int num2 = (int)valstack.Pop();
                                string op = (string)opstack.Pop();
                                valstack.Push(math(num2, op, num1));
                            }
                        }
                    }
                    else {
                        throw new Exception("less an 2 var in valstack"); }
                }
                }
            //Console.WriteLine($"Final state: valstack: [{string.Join(", ", valstack)}], opstack: [{string.Join(", ", opstack.Cast<object>())}]");

            // Process any remaining operators
            while (opstack.Count > 0 && valstack.Count == 2)
            {
                int num1 = (int)valstack.Pop();
                int num2 = (int)valstack.Pop();
                string op = (string)opstack.Pop();
                valstack.Push(math(num2, op, num1));
            }

            //return the value
            if (valstack.Count == 1 && opstack.Count == 0)
            {
                int finalVal = (int)valstack.Pop();
                return finalVal;
            }
            else
            {
                throw new Exception("Invalid expression or incomplete evaluation");
            }
        }
        
        private static int math(int num1, string op, int num2)
        {
            int value = 0;
            if (op == "+") { value = num1 + num2;}
            else if (op == "-") {  value = num1 - num2;}
            else if (op == "*") {  value = num1 * num2;}
            else if (op == "/") { if (num2 == 0) { throw new DivideByZeroException("Cannot divine by zero"); } else { value = num1 / num2; } } 
            return value;
        }
    }
    }
