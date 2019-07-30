using System;
using System.Reflection;
using System.Collections.Generic;

public static class Functions {

    public static Func<double, double, double> Test;
    //public static Func<double,double,double> F;
    //public static Func<double,double> V;
    //public static Func<double,double> U0;

    public static Dictionary<string,MethodInfo> Methods = new Dictionary<string, MethodInfo>();

	public static void Init()
	{
	    Test = Delegate.CreateDelegate(typeof(Func<double, double, double>), Methods[nameof(Test)]) as Func<double, double, double>;
	    //F = Delegate.CreateDelegate (typeof(Func<double,double,double>), Methods["F"]) as Func<double,double,double>;
	    //V = Delegate.CreateDelegate (typeof(Func<double,double>), Methods["V"]) as Func<double,double>;
	    //U0 = Delegate.CreateDelegate (typeof(Func<double,double>), Methods["U0"]) as Func<double,double>;
	}
}
