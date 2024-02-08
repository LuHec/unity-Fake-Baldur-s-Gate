using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Ga_Attack : AbilityBase
{
    private bool attackEnd;

    public Ga_Attack(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        abilityName = "Ga_Attack";
        indicator = new CircularIndicator(this);
    }

    protected override async UniTask AbilityTask(TargetData targetData)
    {
        await Attack(targetData);
    }

    private async UniTask Attack(TargetData targetData)
    {
        var targets = targetData.targets;
        // var targets = new List<GameActor>(targetData.targets);
        foreach (var target in targets)
        {
            var endAttackSource = new UniTaskCompletionSource();
            owner.Attack(target, () =>
            {
                endAttackSource.TrySetResult();
            });
 
            await endAttackSource.Task;

            owner.abilitySystem.TryApplyModifier(
                ModifierPool.Instance.CreateModifier(
                    "Mo_Decrease_Hp",
                    owner,
                    target));
        }
    }
}