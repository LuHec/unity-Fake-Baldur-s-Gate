using System;
using System.Collections;
using UnityEngine;

public class AttackActorCommand : CommandInstance
{
    // 攻击者
    // private GameActor _actorAttacker;
    private bool coroutineStarted = false;

    // 被攻击者
    private GameActor _actorAttacked;
    float t = 0;

    public AttackActorCommand(GameActor actorAttacked)
    {
        _actorAttacked = actorAttacked;
    }

    public override bool Excute(GameActor attacker, Action onExcuteFinsihed)
    {
        if (attacker == _actorAttacked) return false;
        

        Attacking(attacker, _actorAttacked, onExcuteFinsihed);

        return true;
    }

    public override void Undo(GameActor actor)
    {
    }

    /// <summary>
    /// 攻击函数，时间内不可操作
    /// </summary>
    /// <param name="OnAttackFinished"></param>
    public void Attacking(GameActor attacker, GameActor actorAttacked, Action OnAttackFinished)
    {
        if (t < 2f)
        {
            t += Time.fixedDeltaTime;
            Debug.Log("Attacking");
        }
        else
        {
            Debug.Log("inner");
            attacker.Attack(actorAttacked);
            actorAttacked.transform.transform.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            OnAttackFinished();
        }
    }
}