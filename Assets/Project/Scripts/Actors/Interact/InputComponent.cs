using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputComponent
{
    #region Params

    // 初始化参数
    private Camera mainCamera;
    private PlayerInput playerInput;

    // 交互对象
    public GameActor CurrentControlActor =>
        ActorsManagerCenter.Instance.GetActorByDynamicId(TurnManager.Instance.GetCurrentPlayerId());

    private GameActor currentInteractActor;
    
    public GameActor IsOnActor()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInput.Instance.MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, LayerMask.GetMask("Actor")))
        {
            return raycastHit.transform.GetComponent<GameActor>();
        }

        return null;
    }

    #endregion

    #region GameLogic

    public InputComponent()
    {
        mainCamera = Camera.main;
        playerInput = PlayerInput.Instance;
    }

    /// <summary>
    /// 处理所有输入，只有当前控制角色处在自由模式，或者是当前回合执行对象时，才会产生新的命令
    /// </summary>
    public void UpdateInput()
    {
        if (PlayerInput.Instance.IsLClick && !EventSystem.current.IsPointerOverGameObject())
        {
            // 左键时，需要关闭交互窗口
            UIPanelManager.Instance.HidePanel<ActorInteractPanel>();
            
            Vector3 position = playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
            CurrentControlActor.moveComponent.SetTarget(position);
        }

        if (PlayerInput.Instance.IsRClick)
        {
            GameActor actor = IsOnActor();
            if (actor != null)
            {
                UIPanelManager.Instance.ShowPanel<ActorInteractPanel>()
                    .UpdatePanel(PlayerInput.Instance.MousePos, actor.DynamicId);
            }
        }
    }

    #endregion
}