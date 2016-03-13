# GP4Sim.CSharpAgents
Contains classes enabling to export evolved GP agents as C# source code.

`CSharpFormatter` features the core parser translating the syntax tree representing the GP individual into a linearized form and ultimately to C# source code.

## Usage
```
CSharpFormatter formatter = new CSharpFormatter();
string source = formatter.FormatFull(solution.Model.SymbolicExpressionTree, solution.ActualInputVector);
File.WriteAllText(filename, source);
```
