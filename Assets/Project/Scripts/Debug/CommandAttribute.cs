using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
}

public static class CommandInvoker
{
    public static void InvokeCommand(Type type, string functionName, object[] argvs)
    {
        MethodInfo methodInfo = type.GetMethod(functionName);

        if (methodInfo != null && methodInfo.IsDefined(typeof(CommandAttribute)))
            methodInfo.Invoke(null, argvs);
        else
        {
            RunTimeDebugger.Instance.LogMessage("Wrong Command!");
        }
    }

    public static void InvokeCommand(object obj, string functionName, object[] argvs)
    {
        MethodInfo methodInfo = obj.GetType().GetMethod(functionName);

        if (methodInfo != null && methodInfo.IsDefined(typeof(CommandAttribute)))
            methodInfo.Invoke(obj, argvs);
        else
        {
            RunTimeDebugger.Instance.LogMessage("Wrong Command!");
        }
    }
}