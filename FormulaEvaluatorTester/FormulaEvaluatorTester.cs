// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;
using System;

namespace FormulaEvaluatorTester
{
    class Program
    {
        static void Main()
        {
            // Define a variable evaluator delegate
            Evaluator.Lookup variableEvaluator = VariableLookup;

            // Test expressions
            TestExpression("(2 + 3) * 5 + 2", variableEvaluator);
            TestExpression("(2 + X1) * 5 + 2", variableEvaluator);
            TestExpression("(10 / 2) + (3 * 4)", variableEvaluator);
            TestExpression("5 * (2 + 3)", variableEvaluator);

            Console.ReadLine();
        }

        static void TestExpression(string expression, Evaluator.Lookup variableEvaluator)
        {
            try
            {
                double result = Evaluator.Evaluate(expression, variableEvaluator);
                Console.WriteLine($"Expression: {expression} = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating expression '{expression}': {ex.Message}");
            }
        }

        // Sample variable lookup method
        static int VariableLookup(string variableName)
        {
            // Provide values for variables
            if (variableName == "X1") return 7;
            else
            {
                // Handle other variables if needed
                throw new Exception($"Variable {variableName} not found");
            }
        }
    }
}

