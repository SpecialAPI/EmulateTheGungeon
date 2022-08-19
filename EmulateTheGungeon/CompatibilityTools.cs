global using static CompatibilityTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public static class CompatibilityTools
{
    public static readonly Assembly mtgAssembly = typeof(ETGModCompatibility).Assembly;

    public static Type GetMTGType(string name)
    {
        return mtgAssembly.GetType(name);
    }

    public static object InvokeMethod(this Type type, string name, object obj, params object[] args)
    {
        try
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Invoke(obj, args);
        }
        catch
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic, null, args.Select(x => x.GetType()).ToArray(), null).Invoke(obj, args);
        }
    }

    public static object InvokeMethod(this object obj, string name, params object[] args)
    {
        return obj.GetType().InvokeMethod(name, obj, args);
    }

    public static object GetFieldValue(this Type type, string name, object obj)
    {
        return type.GetField(name).GetValue(obj);
    }

    public static object GetStaticField(this Type type, string name)
    {
        return type.GetField(name).GetValue(null);
    }

    public static void SetStaticField(this Type type, string name, object value)
    {
        type.GetField(name).SetValue(null, value);
    }

    public static object GetField(this object obj, string name)
    {
        return obj.GetType().GetFieldValue(name, obj);
    }

    public static object InvokeStaticMethod(this Type type, string name, params object[] args)
    {
        if (type.Name == "ETGModConsole" && name == "Log")
        {
            return type.GetMethod(name, new Type[] { typeof(string), typeof(bool) }).Invoke(null, args);
        }
        try
        {
            return type.GetMethod(name).Invoke(null, args);
        }
        catch
        {
            return type.GetMethod(name, args.Select(x => x.GetType()).ToArray()).Invoke(null, args);
        }
    }

    public static object InvokeStaticMTGMethod(string typeName, string methodName, params object[] args)
    {
        return GetMTGType(typeName).InvokeStaticMethod(methodName, args);
    }

    public static object GetStaticMTGField(string typeName, string name)
    {
        return GetMTGType(typeName).GetStaticField(name);
    }

    public static void SetStaticMTGField(string typeName, string name, object value)
    {
        GetMTGType(typeName).SetStaticField(name, value);
    }

    public static object InvokeMTGMethod(string typeName, string methodName, object obj, params object[] args)
    {
        return GetMTGType(typeName).InvokeMethod(methodName, obj, args);
    }
}
