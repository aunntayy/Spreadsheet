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

        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            Stack numstack = new();
            Stack opstack = new();


            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

            // Check each substring 
            foreach (string substring in substrings) {
                //if t = "+"
                if (substring == "+")
                {
                    double num1 = (double)numstack.Pop();
                    double num2 = (double)numstack.Pop();
                    opstack.Pop();
                    numstack.Push(num1 - num2);
                    opstack.Push(substring);
                }
                //if t = "-"
                else if (substring == "-")
                {
                    double num1 = (double)numstack.Pop();
                    double num2 = (double)numstack.Pop();
                    opstack.Pop();
                    numstack.Push(num1 - num2);
                    opstack.Push(substring);
                }
                //if t = "*", "/", "("
                else if (substring == "*" || substring == "/" || substring == "(")
                {
                    opstack.Push(substring);
                }
                //if t is an integer
                else if (int.TryParse(substring, out int n)) {
                    if (opstack.Peek() == "*" || opstack.Peek() == "/")
                    {
                        //For mutiply
                        if (opstack.Peek() == "*")
                        {
                            double num1 = (double)numstack.Pop();
                            numstack.Push(num1 * n);
                        }
                        //For divinding
                        else {
                            double num1 = (double)numstack.Pop();
                            numstack.Push(num1 / n);
                        }
                    }
                }
                // if t = ")"
                else if (substring == ")") {
                    //if top is +
                    if (opstack.Peek() == "+") {
                    double num1 = (double)numstack.Pop();
                    double num2 = (double)numstack.Pop();
                    opstack.Pop();
                    numstack.Push(num1 + num2);
                    }
                    //if top is - 
                    else {
                        double num1 = (double)numstack.Pop();
                        double num2 = (double)numstack.Pop();
                        opstack.Pop();
                        numstack.Push(num1 - num2);
                    }
                    //the top should be "(", pop it
                    opstack.Pop();
                    // If * or / is at the top
                    if (opstack.Peek() == "*")
                    {
                        double num1 = (double)numstack.Pop();
                        double num2 = (double)numstack.Pop();
                        opstack.Pop();
                        numstack.Push(num1 * num2);
                    }
                    else {
                        double num1 = (double)numstack.Pop();
                        double num2 = (double)numstack.Pop();
                        opstack.Pop();
                        numstack.Push(num1 / num2);
                    }
                }
            }
          }
    }
    }
