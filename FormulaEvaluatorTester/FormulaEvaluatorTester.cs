// See https://aka.ms/new-console-template for more information

try
{
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4+2", null)) == 6);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4-2", null)) == 2);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("2-4", null)) == -2);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4/0", null)) == 0);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("4/2", null)) == 2);
    Console.WriteLine((FormulaEvaluator.Evaluator.Evaluate("(4-2)*2", null)) == 4);

}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
