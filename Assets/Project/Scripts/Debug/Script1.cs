using UnityEngine;

public static class Script1
{
    [Command]
    public static void Test()
    {
        RunTimeDebugger.Instance.LogMessage("Hello World!");
    }

    [Command]
    public static void AddBot()
    {

        var position = ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId())
            .transform.position;
        position.x += Random.Range(5, 10);
        position.z += Random.Range(5, 10);
        
        RunTimeDebugger.Instance.LogMessage("Add at " + position);

        TurnManager.Instance.AddFreeModeActorById(ActorsManagerCenter.Instance.LoadActorTest(position));
    }

    [Command]
    public static void Clear()
    {
        RunTimeDebugger.Instance.Flush();
    }
}