// See https://aka.ms/new-console-template for more information

using FormulaEvaluator;

try
{
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4 + 2", null)) == 6);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4-2", null)) == 2);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(2)", null)) == 2);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4/2", null)) == 2);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(3-1)*2", null)) == 4);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("2*(3-1)", null)) == 4);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(3-1)/2", null)) == 1);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(6/2)*2", null)) == 6);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(6)", null)) == 6);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(2+3)/5", null)) == 1);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("2+1*4", null)) == 6);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(2+1)*4", null)) == 12);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("2/1*4", null)) == 8);
    //Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("-2+4", null)) == 2);
    // Define a Lookup delegate to provide values for variables
    FormulaEvaluator.Evaluator.Lookup variableLookup = (variableValue) =>
    {
        if (variableValue == "x") return 2;
        else if (variableValue == "y")
        {
            return 3;
        }
        else if (variableValue == "A200") { return 3; }
        else throw new ArgumentException($"Variable {variableValue} not found.");
    };
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("x*y", variableLookup)) == 6);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("x+y", variableLookup)) == 5);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("x/y", variableLookup)) == 0);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(x+A200)*2/5", variableLookup)) == 2);
    // Test the expression with variables



}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
