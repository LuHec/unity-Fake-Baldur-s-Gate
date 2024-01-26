using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Type = EActorTargetConfirmation.Type;

public class IndicatorBase
{
    // ------------------------------------------------------------------------------
    // Delegates
    public Action onTargetingStart;
    public Action onTargetingCancelled;
    public Action onTargetingValid;
    public Action onTargetingInvalid;

    public EventHandler<ActorTargetData> targetDataReadyDelegate;

    // ------------------------------------------------------------------------------
    // Parameters
    public GameActor owner;
    public HashSet<GameActor> targetsSet = new HashSet<GameActor>();
    protected int maxTargetsCount;
    protected Vector3 indicatorPosition = Vector3.zero;
    protected float maxDistance = 10f;

    protected Type targetConfirmationType;
    protected bool bTargetReady = false;

    public IndicatorBase(GameActor owner)
    {
        this.owner = owner;
    }

    // ------------------------------------------------------------------------------
    // Function Base
    public virtual async UniTask WaitTargetData(AbilityBase ability, EActorTargetConfirmation.Type confirmType)
    {
        bTargetReady = false;
        targetConfirmationType = confirmType;

        await Tick();
    }
    
    // Can be called by outside
    public void CancelTargeting()
    {
        onTargetingCancelled?.Invoke();
    }
    
    public void RemoveTarget()
    {
        // 移除目标
    }

    public bool AddTarget(GameActor target)
    {
        if (target == null || target == owner)
        {
            onTargetingInvalid?.Invoke();
            return false;
        }

        targetsSet.Add(target);
        return true;
    }

    public void EndTargeting()
    {
        targetsSet.Clear();
        indicatorPosition.x = indicatorPosition.y = indicatorPosition.z = 0;
    }

    // ------------------------------------------------------------------------------
    // Function needs override
    // 确认选中
    protected virtual void ConfirmTargetAndContinue()
    {
    }

    protected virtual async UniTask Tick()
    {
        await UniTask.Yield();
    }

    protected virtual void GetPlayerMouseWorldPositionAndConfirm(InputAction.CallbackContext context)
    {
        Vector3 worldPosition;
        PlayerInput playerInput = PlayerInput.Instance;
        Ray ray = playerInput.MainCamera.ScreenPointToRay(playerInput.MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask("Default")))
        {
            GameActor target = raycastHit.transform.GetComponent<GameActor>();
            if (AddTarget(target))
            {
                ConfirmTargetAndContinue();
            }
        }
    }

    public void MoveIndicator(Vector3 position)
    {
        indicatorPosition = position;
    }
}