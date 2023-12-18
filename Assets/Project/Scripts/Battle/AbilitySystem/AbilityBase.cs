using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 命名方式：Ga_XXX 
/// </summary>
public abstract class AbilityBase
{
    public float timer;
    public string name;
    public GameActor owner;
    public AbilitySystem abilitySystem;

    // 技能激活时要做的
    public Action onActive;

    // 技能结束时要做的
    public Action onFinished;
    public bool isRunning = true;

    protected AbilityBase(GameActor owner)
    {
        this.owner = owner;

        var character = (Character)owner;
        abilitySystem = character.abilitySystem;

        onActive += OnActive;
        onFinished += OnFinished;
        onFinished += () => { isRunning = false; };
    }

    public abstract void OnActive();
    public abstract void OnFinished();

    /// <summary>
    /// 激活能力，通过回调函数同步。所有的任务都写在这里面
    /// </summary>
    /// <param name="onAbilityEnd">能力完成后的回调函数</param>
    public abstract void ActiveAbility(Action onAbilityEnd = null);
}