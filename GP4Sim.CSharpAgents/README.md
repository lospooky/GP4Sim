# GP4Sim.CSharpAgents
Contains classes enabling to export evolved GP agents as C# source code.

`CSharpFormatter` features the core parser translating the syntax tree representing the GP individual into a linearized form and ultimately to C# source code.

`CSStrings` contains all the boilerplate (usings, declarations, ...)  necessary to produce a valid, compilable C# file from the output of CSharpFormatter. Edit to your own taste.

## Usage
Just construct the object and use the `FormatFull` method passing the `SymbolicExpressionTree` and the `ActualInputVector` objects (containing variable mappings) as argument.<br>
Do stuff with the resulting string, e.g., write it to file or invoke the compiler on it!<br>
Example:
```C#
CSharpFormatter formatter = new CSharpFormatter();
string source = formatter.FormatFull(solution.Model.SymbolicExpressionTree, solution.ActualInputVector);
File.WriteAllText(filename, source);
```
