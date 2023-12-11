using System;
using System.Collections;
using UnityEngine;

public class AttackActorCommand : CommandInstance
{
    private bool hasAttacked = false;

    // 攻击者
    private GameActor actorAttacker;
    // 被攻击者
    private GameActor actorAttacked;

    public AttackActorCommand(GameActor actorAttacked)
    {
        this.actorAttacked = actorAttacked;
    }

    public override bool Excute(GameActor attacker, Action onExcuteFinsihed)
    {
        if (attacker == actorAttacked) return false;

        if (!hasAttacked)
        {
            hasAttacked = true;
            Attacking(attacker, actorAttacked, onExcuteFinsihed);
        }

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
        attacker.Attack(actorAttacked, () =>
        {
            OnAttackFinished?.Invoke();
            IsRunning = false;
        });
    }
}