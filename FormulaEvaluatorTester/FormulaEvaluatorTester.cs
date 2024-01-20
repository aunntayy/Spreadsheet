// See https://aka.ms/new-console-template for more information
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
///   This is the test for FormulaEvaluator
///    
/// </summary>

using System;

public class FormulaEvaluatorTest
{
    public static void Main()
    {
        TestBasicMath();
        TestMultiLayerMath();
        TestVariablesWithAssignedValue();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expected_result"></param>
    /// <param name="variableLookup"></param>
    private static void TestExpression(string input, int expected_result, FormulaEvaluator.Evaluator.Lookup variableLookup = null)
    {
        try
        {
            int result = FormulaEvaluator.Evaluator.Evaluate(input, variableLookup);
            Console.WriteLine(result == expected_result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    /// <summary>
    ///  Test for basic math
    /// </summary>
    private static void TestBasicMath()
    {
        Console.WriteLine("Testing Basic Math:");
        TestExpression("5 + 2", 7);
        TestExpression("5-2", 3);
        TestExpression("2", 2);
        TestExpression("4/2", 2);
        TestExpression("4-2",2);
        TestExpression("", 0);

        // Invalid expression
        TestExpression("-2", -2);
        TestExpression("25*",25);
    }
    /// <summary>
    /// Test for multilayerMath
    /// </summary>
    private static void TestMultiLayerMath()
    {
        Console.WriteLine("Testing Multi-Layer Math:");
        TestExpression("(3-1)*2", 4);
        TestExpression("2*(3-1)", 4);
        TestExpression("(3-1)/2", 1);
        TestExpression("(6/2)*2", 6);
        TestExpression("(6)", 6);
        TestExpression("(2+3)/5", 1);
        TestExpression("2+1*4", 6);
        TestExpression("(2+1)*4", 12);
        TestExpression("2/1*4", 8);
        TestExpression("2 + 4", 6);
        // Invalid expression
        TestExpression("()0", 0);
        TestExpression("(2+2(", 6);
        TestExpression("2_", 8);
        TestExpression("(2*3))", 6);

    }
    /// <summary>
    /// Test Varible with assigned value 
    /// </summary>
    /// <exception cref="Exception"></exception>
    private static void TestVariablesWithAssignedValue()
    {
        Console.WriteLine("Testing Variables with Assigned Value:");
        FormulaEvaluator.Evaluator.Lookup variableLookup = (variableValue) =>
        {
            if (variableValue == "X1") return 2;
            else if (variableValue == "Y1") return 3;
            //wrong variable name
            else if (variableValue == "A200B1") return 3;
            else if (variableValue == "2A") return 3;
            else if (variableValue == "AA1") return 3;
            else throw new Exception($"Variable {variableValue} not found.");
        };

        TestExpression("X1*Y1", 6, variableLookup);
        TestExpression("X1+Y1", 5, variableLookup);
        TestExpression("X1/Y1", 0, variableLookup);

        //Wrong variable name and unassigned variable
        TestExpression("(X1+A200B1)*2/5", 2, variableLookup);
        TestExpression("(X1+A1)*2/5", 2, variableLookup);
        TestExpression("(X1+AA1)*2/5", 2, variableLookup);
        TestExpression("(X1+2A)*2/5", 2, variableLookup);
    }
}
