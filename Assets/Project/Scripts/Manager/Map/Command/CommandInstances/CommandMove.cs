using LuHec.Utils;
using UnityEngine;

public class CommandMove : CommandInstance
{
    public override void Excute(GameActor actor)
    {
        Move(actor);
    }

    public override void Undo(GameActor actor)
    {
        
    }

    void Move(GameActor actor)
    {
        Vector3 worldPos = PlayerInput.Instance.GetMouse3DPositionNew("Default");
        actor.Move(worldPos);
    }
}