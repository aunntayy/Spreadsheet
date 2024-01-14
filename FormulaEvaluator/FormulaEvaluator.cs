using System.Collections;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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
    ///    [This library evaluates arithmetic expressions using standard infix notation]
    ///    
    /// </summary>

    public static class Evaluator
    {
        public delegate int Lookup(String variable_name);

        public static double Evaluate(String expression, Lookup variableEvaluator)
        {
            Stack valstack = new();
            Stack opstack = new();


            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Check each substring 
            foreach (string substring in substrings) {
                //if t = "+" or "-"
                if (substring == "+" || substring == "-")
                {
                    if (valstack.Count >= 2)
                    {
                        double num1 = (double)valstack.Pop();
                        double num2 = (double)valstack.Pop();
                        string op = (string)opstack.Pop();
                        valstack.Push(math(num1, op, num2));
                        opstack.Push(substring);
                    }
                    else { throw new Exception("less an 2 var in valstack"); }
                }
                //if t = "*", "/", "("
                else if (substring == "*" || substring == "/" || substring == "(")
                {
                    opstack.Push(substring);
                }
                //if t is an integer
                else if (int.TryParse(substring, out int n)) {
                    if (valstack.Count == 0)
                    {
                        //For mutiply and divine
                        if (opstack.Peek() == "*" || opstack.Peek() == "/")
                        {
                            double num1 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, n));
                        }
                        //push t onto value stack
                        else { valstack.Push(n); }
                    }
                    else { throw new Exception("Empty value stack"); }
                }
                // if t = ")"
                else if (substring == ")") {
                    if (valstack.Count >= 2)
                    {
                        //if top is +
                        if (opstack.Peek() == "+" || opstack.Peek() == "-")
                        {
                            double num1 = (double)valstack.Pop();
                            double num2 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, num2));
                            if (opstack.Peek() == "(") { opstack.Pop(); } //the top should be "(", pop it
                            else { throw new Exception("The top is not ( "); };
                        }
                    }
                    else { throw new Exception("less an 2 var in valstack"); }
                    if (valstack.Count >= 2)
                    {
                        // If * or / is at the top
                        if (opstack.Peek() == "*" || opstack.Peek() == "-")
                        {
                            double num1 = (double)valstack.Pop();
                            double num2 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, num2));
                        }
                    }
                    else { throw new Exception("less an 2 var in valstack"); }
                }
                //if t is a variable
                else if (Regex.IsMatch(substring, @"[a-zA-Z]+\d+"))
                {
                    if (valstack.Count == 0)
                    {
                        // Handle variables using the variableEvaluator delegate
                        double variableValue = variableEvaluator(substring);
                        //For mutiply and divine
                        if (opstack.Peek() == "*" || opstack.Peek() == "/")
                        {
                            double num1 = (double)valstack.Pop();
                            string op = (string)opstack.Pop();
                            valstack.Push(math(num1, op, n));
                        }
                        //push t onto value stack
                        else { valstack.Push(n); }
                    }
                    else { throw new Exception("Empty value stack"); }
                }
                }
            //return the value
            if (valstack.Count == 1 && opstack.Count == 0)
            {
                return (double)valstack.Pop()haha;
            }
          }
        
        private static double math(double num1, string op, double num2)
        {
            double value = 0;
            if (op == "+") { value = num1 + num2;}
            else if (op == "-") {  value = num1 - num2;}
            else if (op == "*") {  value = num1 * num2;}
            else if (op == "/") { if (num2 == 0) { throw new DivideByZeroException("Cannot divine by zero"); } else { value = num1 / num2; } } 
            return value;
        }
    }
    }
