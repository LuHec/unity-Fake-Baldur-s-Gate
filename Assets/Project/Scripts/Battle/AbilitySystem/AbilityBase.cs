using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 命名方式：Ga_XXX 
/// </summary>
public class AbilityBase
{
    public float timer;
    public string name;
    public Character owner;
    public AbilitySystem abilitySystem;

    // 技能激活时要做的
    public Action onActive;
    // 技能结束时要做的
    public Action onFinished;

    public AbilityBase(AbilitySystem abilitySystem)
    {
        this.abilitySystem = abilitySystem;
        this.owner = abilitySystem.Owner;
    }

    public async void ActiveAbility()
    {
        onActive?.Invoke();
        await AbilityTask();
        onFinished?.Invoke();
    }

    protected virtual async UniTask AbilityTask()
    {
        await UniTask.Yield();
    }
}