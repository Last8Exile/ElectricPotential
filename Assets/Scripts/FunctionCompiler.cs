using System.CodeDom.Compiler;
using UnityEngine;
using System.Text;
using Microsoft.CSharp;

public class FunctionCompiler : MonoBehaviour
{

    public void CompileFunction(string code)
    {
        if (code.Length <= 0) return;

        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters();

        parameters.GenerateInMemory = true;
        parameters.GenerateExecutable = false;

        var results = provider.CompileAssemblyFromSource(parameters, code);

        if (results.Errors.HasErrors)
        {
            var sb = new StringBuilder();

            foreach (CompilerError error in results.Errors)
            {
                sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
            }

            Debug.Log(sb.ToString());
            return;
        }
        Debug.Log("Compile Succes");
        var assembly = results.CompiledAssembly;

        var types = assembly.GetTypes();
        if (types.Length > 0)
        {
            Functions.Methods.Clear();
            foreach (var function in MonoSingleton<FunctionBuilder>.Instance.Functions)
                Functions.Methods.Add(function.Name, types[0].GetMethod(function.Name));
        }
        Functions.Init();
    }

}
