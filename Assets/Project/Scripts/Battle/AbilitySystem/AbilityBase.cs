using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

/// <summary>
/// 命名方式：Ga_XXX 
/// </summary>
public class AbilityBase
{
    // ------------------------------------------------------------------------------
    // Params
    public float timer;
    public string abilityName;
    public Character owner;
    public AbilitySystem abilitySystem;
    public IndicatorBase indicator;

    public AbilityBase(AbilitySystem abilitySystem)
    {
        this.abilitySystem = abilitySystem;
        this.owner = abilitySystem.Owner;

        onAbilityActive += PrepareCachedData;
        onAbilityFinished += ClearCachedData;
        onAbilityCanceled += ClearCachedData;
    }

    // ------------------------------------------------------------------------------
    // Bind Input
    public void AbilityBindInput(ref EventHandler<EventArgsType.PlayerConfirmMessage> confirmHandler,
        ref EventHandler<EventArgsType.PlayerCancelMessage> cancelHandler, ref EventHandler<Vector3> positionHandler)
    {
        indicator.BindInput(ref confirmHandler, ref cancelHandler, ref positionHandler);
    }

    // ------------------------------------------------------------------------------
    // Delegates
    public Action onAbilityActive;
    public Action onAbilityFinished;
    public Action onAbilityCanceled;

    // 当收到消息后会退出WaitTargetData状态
    private UniTaskCompletionSource<TargetData> targetDataCompletionSource;

    public void ListenTargetData(Object sender, TargetData targetData)
    {
        targetDataCompletionSource.TrySetResult(targetData);
    }

    // 等待targetData信息
    protected async UniTask<TargetData> WaitTargetData()
    {
        targetDataCompletionSource = new UniTaskCompletionSource<TargetData>();
        return await targetDataCompletionSource.Task;
    }


    // ------------------------------------------------------------------------------
    // Run Functions
    protected TargetData cachedTargetData;

    // 可以重写为不需要TargetActor的技能
    public virtual async void ActiveAbility()
    {
        // 寻敌时禁止移动
        owner.moveComponent.DisableMove();
        
        indicator.targetDataReadyHandler += ListenTargetData;
        indicator.ReadyToActive();
        cachedTargetData = await WaitTargetData();
        Debug.Log("Get Message");
        indicator.targetDataReadyHandler -= ListenTargetData;
        
        owner.moveComponent.EnableMove();

        if (cachedTargetData.bConfirm)
        {
            abilitySystem.CommitAbilityTask(this, abilityName);
        }
        else
        {
            onAbilityCanceled?.Invoke();
        }
    }

    public virtual async void ExecuteAbility()
    {
        owner.moveComponent.DisableMove();
        await AbilityTask(cachedTargetData);
        owner.moveComponent.EnableMove();
        Debug.Log("Ability Finished");
        // cachedTargetData = null;
        onAbilityFinished?.Invoke();
    }

    protected virtual async UniTask AbilityTask(TargetData targetData)
    {
        await UniTask.Yield();
    }

    protected void PrepareCachedData()
    {
    }

    protected void ClearCachedData()
    {
        cachedTargetData = null;
    }
}