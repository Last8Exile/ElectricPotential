using System;
using UnityEngine;

public class FunctionBuilder : MonoBehaviour
{
	[Serializable]
	public struct Function
	{
		public string Name;
		public string Args;
		public string Code;
	}

	public Function[] Functions;

	private string mFuncStart = "\n\tpublic static double ";
	private string mFuncMed = " \n\t{\n\t\treturn ";
	private string mFuncEnd = ";\n\t}";
	private string mBegin ="using static System.Math;\n\npublic static class Function {\n\t";
	private string mEnd = "\n}";

	public void BuildAndCompileNewFunctions()
	{
		MonoSingleton<FunctionCompiler>.Instance.CompileFunction (BuildFunction ());
	}

	public string BuildFunction()
	{
		var functions = "";
		foreach (var func in Functions)
			functions += GetFunction (func);
		return mBegin + functions + mEnd;
	}

	private string GetFunction(Function func)
	{
		return mFuncStart + func.Name + "(" + func.Args + ")" + mFuncMed + func.Code + mFuncEnd;
	}
}
