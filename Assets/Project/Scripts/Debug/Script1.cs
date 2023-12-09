public static class Script1
{
    [Command]
    public static void Test()
    {
        RunTimeDebugger.Instance.LogMessage("Hello World!");
    }
}