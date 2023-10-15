using System;
using UnityEngine;

public class AttackActorCommand : CommandInstance
{
    // 攻击者
    // private GameActor _actorAttacker;

    // 被攻击者
    private GameActor _actorAttacked;

    public AttackActorCommand(GameActor actorAttacked)
    {
        _actorAttacked = actorAttacked;
    }

    public override bool Excute(GameActor attacker, Action onExcuteFinsihed)
    {
        if (attacker == _actorAttacked) return false; 
        _actorAttacked.transform.transform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        onExcuteFinsihed();
        return true;
    }

    public override void Undo(GameActor actor)
    {
    }
}