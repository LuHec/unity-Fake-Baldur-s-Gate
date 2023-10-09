using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : Singleton<PlayerInput>
{
    private PlayerInputSystem _playerInputSystem;

    public Vector2 PlayerMovement => _playerInputSystem.Player.Move.ReadValue<Vector2>();
    public bool IsMove => PlayerMovement.sqrMagnitude != 0;
    public bool IsLPress => _playerInputSystem.Player.Click.IsPressed();
    public bool IsLClick => _playerInputSystem.Player.Click.WasPressedThisFrame();
    public bool IsRClick => _playerInputSystem.Player.RightClick.WasPressedThisFrame();
    public Vector2 Axis => _playerInputSystem.Player.Look.ReadValue<Vector2>().normalized;
    public bool IsEscape => _playerInputSystem.Player.Exit.WasPressedThisFrame();
    public Vector2 MouseScroll => _playerInputSystem.Player.MouseScroll.ReadValue<Vector2>();
    public Vector2 MouseAxis => _playerInputSystem.Player.MouseAxis.ReadValue<Vector2>();
    public Vector2 MousePos => _playerInputSystem.Player.MousePos.ReadValue<Vector2>();
    public Vector2 MouseDelta => _playerInputSystem.Player.MouseDelta.ReadValue<Vector2>();

    protected override void Awake()
    {
        base.Awake();
        _playerInputSystem = new PlayerInputSystem();
    }

    public void EnableGamePlayInputs()
    {
        _playerInputSystem.Player.Enable();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public Vector3 GetMouse3DPosition(int mouseLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(MousePos);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseLayerMask))
        {
#if UNITY_EDITOR
            Debug.Log(raycastHit.point);
#endif
            return raycastHit.point;
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("null");
#endif
            return Vector3.zero;
        }
    }
}