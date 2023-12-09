using System.Globalization;
using System.Text;
using LuHec.Utils;
using Project.Scripts.LuHeUtility;
using UnityEngine;
using Object = System.Object;

public class DebugStringBuffer
{
    public StringBuilder Sbuffer => sBuffer;
    private StringBuilder sBuffer;

    public DebugStringBuffer(string log)
    {
        sBuffer = new StringBuilder("[Log] ", 80);
        sBuffer.Append(LuHeUtility.GetCurrentTime());
        sBuffer.Append("    ");
        sBuffer.Append(log);
    }
    
    public DebugStringBuffer(Object logger, string log)
    {
        sBuffer = new StringBuilder(LuHeUtility.GetCurrentTime(), 80);
        sBuffer.Append("    ");
        sBuffer.Append(logger);
        sBuffer.Append("    ");
        sBuffer.Append(log);
    }

    public override string ToString()
    {
        return sBuffer.ToString();
    }
}