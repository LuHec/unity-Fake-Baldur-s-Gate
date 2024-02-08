using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;
using Type = EActorTargetConfirmation.Type;

public class IndicatorBase
{
    // ------------------------------------------------------------------------------
    // Parameters
    public AbilityBase ownerAbility;
    public List<GameActor> targetsList = new List<GameActor>();
    protected int maxTargetsCount;
    protected float maxDistance = 10f;
    protected GameObject indicatorInstance;
    protected Vector3 indicatorPosition;

    public IndicatorBase(AbilityBase ownerAbility)
    {
        this.ownerAbility = ownerAbility;

        onTargetingCancelled += CancelTargeting;
    }

    // ------------------------------------------------------------------------------
    // Delegates
    public Action onTargetingStart;
    public Action onTargetingCancelled;
    public Action onTargetingValid;
    public Action onTargetingInvalid;

    private EventHandler<EventArgsType.PlayerConfirmMessage> tConfirmHandler;
    private EventHandler<EventArgsType.PlayerCancelMessage> tCancelHandler;
    private EventHandler<Vector3> tMousePositionHandler;
    public EventHandler<TargetData> targetDataReadyHandler;

    public void BindInput(
        ref EventHandler<EventArgsType.PlayerConfirmMessage> confirmHandler,
        ref EventHandler<EventArgsType.PlayerCancelMessage> cancelHandler,
        ref EventHandler<Vector3> mousePositionHandler)
    {
        confirmHandler += TryConfirm;
        cancelHandler += TryCancel;
        mousePositionHandler += MoveIndicator;

        tConfirmHandler = confirmHandler;
        tCancelHandler = cancelHandler;
        tMousePositionHandler = mousePositionHandler;
    }

    protected void UnbindInput()
    {
        tConfirmHandler -= TryConfirm;
        tCancelHandler -= TryCancel;
        tMousePositionHandler -= MoveIndicator;
    }

    private void TryConfirm(Object sender, EventArgsType.PlayerConfirmMessage confirmMessage)
    {
        if (confirmMessage.bOnUI == false)
        {
            ConfirmTargetAndContinue();
        }
    }

    public void TryCancel(Object sender, EventArgsType.PlayerCancelMessage cancelMessage)
    {
        // 发送空的TargetData消息
        if (cancelMessage.bOnUI == false)
        {
            onTargetingCancelled?.Invoke();
        }
    }

    public void CancelTargeting()
    {
        EndTargeting();
        targetDataReadyHandler?.Invoke(this, new TargetData(false, null));
        UnityEngine.Object.Destroy(indicatorInstance);
    }

    public void EndTargeting()
    {
        targetsList.Clear();
    }

    public void RemoveTarget()
    {
        // 移除目标
    }

    public bool AddTarget(GameActor target)
    {
        // 判断tag是否符合
        if (target == null || target == ownerAbility.owner)
        {
            onTargetingInvalid?.Invoke();
            return false;
        }

        targetsList.Add(target);
        return true;
    }


    // ------------------------------------------------------------------------------
    // Function needs override

    // 重写加载的指示器
    public virtual void ReadyToActive()
    {
    }


    // 重写具体的寻敌逻辑
    protected virtual void ConfirmTargetAndContinue()
    {
        targetDataReadyHandler?.Invoke(this, null);
        UnbindInput();
    }

    protected virtual async UniTask Tick()
    {
        await UniTask.Yield();
    }

    public void MoveIndicator(Object sender, Vector3 position)
    {
        MoveIndicator(position);
    }

    public void MoveIndicator(Vector3 position)
    {
        indicatorPosition = position;
        if (indicatorInstance != null) indicatorInstance.transform.position = position;
    }
}