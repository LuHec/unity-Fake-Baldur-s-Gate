using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInput
{
    public static PlayerInput Instance = new PlayerInput();

    private PlayerInput()
    {
        mainCamera = Camera.main;
        playerInputSystem = new PlayerInputSystem();
        EnableGamePlayInputs();
    }

    public PlayerInputSystem.PlayerActions PlayInputAction => playerInputSystem.Player;
    public Camera MainCamera => mainCamera;
    private PlayerInputSystem playerInputSystem;
    private Camera mainCamera;

    public Vector2 PlayerMovement => playerInputSystem.Player.Move.ReadValue<Vector2>();
    public bool IsMove => PlayerMovement.sqrMagnitude != 0;
    public bool IsLPress => playerInputSystem.Player.Click.IsPressed();
    public bool IsLClick => playerInputSystem.Player.Click.WasPressedThisFrame();
    public bool IsRClick => playerInputSystem.Player.RightClick.WasPressedThisFrame();
    public bool IsSpace => playerInputSystem.Player.Space.WasPressedThisFrame();
    public Vector2 Axis => playerInputSystem.Player.Look.ReadValue<Vector2>().normalized;
    public bool IsEscape => playerInputSystem.Player.Exit.WasPressedThisFrame();
    public Vector2 MouseScroll => playerInputSystem.Player.MouseScroll.ReadValue<Vector2>();
    public Vector2 MouseAxis => playerInputSystem.Player.MouseAxis.ReadValue<Vector2>();
    public Vector2 MousePos => playerInputSystem.Player.MousePos.ReadValue<Vector2>();
    public Vector2 MouseDelta => playerInputSystem.Player.MouseDelta.ReadValue<Vector2>();

    public bool Epress => playerInputSystem.Player.E.IsPressed();
    public bool Qpress => playerInputSystem.Player.Q.IsPressed();

    public bool DebugPress => playerInputSystem.Player.Debug.WasPressedThisFrame();

    private Vector3 cachedMousePosition = Vector3.zero;

    public void EnableGamePlayInputs()
    {
        playerInputSystem.Player.Enable();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DisableGamePlayInputs()
    {
        playerInputSystem.Player.Disable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Vector3 GetMouse3DPosition(int mouseLayerMask)
    {
        Ray ray = mainCamera.ScreenPointToRay(MousePos);

        if (Physics.Raycast(ray, out RaycastHit rayCastHit, 999f, mouseLayerMask))
        {
            cachedMousePosition = rayCastHit.point;
#if UNITY_EDITOR
            // Debug.Log("MousePos: " + MousePos + " World Pos: " + cachedMousePosition);
#endif
            return cachedMousePosition;
        }
        else
        {
#if UNITY_EDITOR
            // Debug.Log("null, mouse pos return vector3.zero");
#endif
            return cachedMousePosition;
        }
    }
}