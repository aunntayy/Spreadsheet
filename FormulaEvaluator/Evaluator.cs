using System.Collections;
using System.Collections.Generic;
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
        /// Delegate to look up varible assigned value
        /// </summary>
        /// <param name="variable_name"></param>
        /// <returns></returns>
        public delegate int Lookup(String variable_name);
        /// <summary>
        /// Algorithm for the infix notation
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
                if (Regex.IsMatch(substring, @"[a-zA-Z_]\w*"))
                {
                    if (Regex.IsMatch(substring, @"^[a-zA-Z]+\d+$") == false)
                    { throw new ArgumentException("Not a valid varible name"); }
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
                if (substring == "+" || substring == "-") {
                    if (valstack.Count == 2 && opstack.Count > 0 && (opstack.Peek().Equals("+")  || opstack.Peek().Equals("-"))) {
                        if (opstack.Peek().Equals("+"))
                        {
                            int num1 = (int)valstack.Pop();
                            int num2 = (int)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, num2));
                        }
                        else if (opstack.Peek().Equals("-"))
                        {
                            int num1 = (int)valstack.Pop();
                            int num2 = (int)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(num2 - num1);
                        }
                    }
                    opstack.Push(substring);
                }
                //if t = "*", "/", "("
                if (substring == "*" || substring == "/" || substring == "("){
                    opstack.Push(substring);
                }

                // if t = ")"
                if (substring == ")")
                {
                    // Check if there are operands in the stack
                    if (valstack.Count < 1)
                    {
                        throw new ArgumentException("Less than 1 operand found before closing parenthesis");
                    }

                    // Evaluate expressions inside parentheses
                    while (opstack.Count > 0 && opstack.Peek().ToString() != "(")
                    {
                        int num1 = valstack.Pop();
                        int num2 = valstack.Pop();
                        string op = opstack.Pop().ToString();
                        valstack.Push(math(num2, op, num1));
                    }
                    if (opstack.Count == 0)
                    {
                        throw new ArgumentException("A '(' is not found where expected");
                    }
                    else
                    {
                        opstack.Pop();
                    }

                    // Check if there are additional operators (* or /) after parentheses
                    if (opstack.Count > 0 && (opstack.Peek().ToString().Trim() == "*" || opstack.Peek().ToString().Trim() == "/"))
                    {
                        int num1 = valstack.Pop();
                        int num2 = valstack.Pop();
                        string op = opstack.Pop().ToString();
                        valstack.Push(math(num2, op, num1));
                    }
                }

            }

            // Process any remaining operators
            while (opstack.Count > 0 && valstack.Count >= 2)
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
                throw new ArgumentException("Invalid input or incomplete math work");
            }
        }
        /// <summary>
        /// Method to do the math work depend on the operation
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="op"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        /// <exception cref="DivideByZeroException"></exception>
        private static int math(int num1, string op, int num2)
        {
            int value = 0;
            if (op == "+") { value = num1 + num2;}
            else if (op == "-") {  value = num1 - num2;}
            else if (op == "*") {  value = num1 * num2;}
            else if (op == "/") { if (num2 == 0) { throw new ArgumentException("Cannot divine by zero"); } else { value = num1 / num2; } } 
            return value;
        }
    }
    }
