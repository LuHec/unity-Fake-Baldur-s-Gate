using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  顶层系统
/// </summary>
public class TopSystem : MonoBehaviour
{
    private CommandCenter _commandCenter;
    private PlayerInput _playerInput;

    /// <summary>
    /// 初始化系统
    /// </summary>
    private void Awake()
    {
        _commandCenter = new CommandCenter();
        _playerInput = GetComponent<PlayerInput>();

        _playerInput.EnableGamePlayInputs();
    }

    private void Update()
    {
           
    }
}
