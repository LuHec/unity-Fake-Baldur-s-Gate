using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController
{
    // ------------------------------------------------------------------------------
    // Private Params


    // ------------------------------------------------------------------------------
    // Out Delegates
    // 对外事件触发器，发送UI、指示器相关的信息
    public EventHandler<EventArgsType.PlayerConfirmMessage> playerConfirmHandler;
    public EventHandler<EventArgsType.PlayerCancelMessage> playerCancelHandler;
    public EventHandler<Vector3> playerMousePositionHandler;
    
    public PlayerController()
    {
        // 注册对外的事件触发器
        PlayerInput.Instance.PlayInputAction.Click.started += OnLeftClick;
        PlayerInput.Instance.PlayInputAction.RightClick.started += OnRightClick;
        PlayerInput.Instance.PlayInputAction.MousePos.performed += OnMouseMove;
    }

    /// <summary>
    /// 当按下左键时，会发送确认消息，由订阅者接受(比如技能指示器)
    /// </summary>
    /// <param name="ctx"></param>
    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        Debug.Log("Confirm message");
        playerConfirmHandler?.Invoke(this,
            new EventArgsType.PlayerConfirmMessage(EventSystem.current.IsPointerOverGameObject()));
    }

    public void OnRightClick(InputAction.CallbackContext ctx)
    {
        playerCancelHandler?.Invoke(this,
            new EventArgsType.PlayerCancelMessage(EventSystem.current.IsPointerOverGameObject()));
    }

    public void OnMouseMove(InputAction.CallbackContext ctx)
    {
        if (playerMousePositionHandler != null)
        {
            var position = PlayerInput.Instance.GetMouse3DPosition(LayerMask.GetMask("Default") | LayerMask.GetMask("Actor"));
            playerMousePositionHandler.Invoke(this, position);
        }
    }
}