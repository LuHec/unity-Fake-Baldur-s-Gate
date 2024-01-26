using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ga_Attack : AbilityBase
{
    private bool attackEnd;

    public Ga_Attack(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        name = "Ga_Attack";
    }

    protected override async UniTask AbilityTask()
    {
        await Attack();
    }

    private async UniTask Attack()
    {
        bool endAttack = false;
        Debug.Log("StartAttack");
        owner.Attack(owner, () => { endAttack = true; });
        while (!endAttack)
        {
            await UniTask.Yield();
        }
        Debug.Log("EndAtk");

        owner.abilitySystem.TryApplyModifier(ModifierPool.Instance.CreateModifier("Mo_Decrease_Hp", owner, owner));
        await UniTask.Yield();
    }
}