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
        _playerInput = PlayerInput.Instance;

        _playerInput.EnableGamePlayInputs();

        _button0 = new CommandMove();
    }

    // Test
    private CommandInstance _button0;
    [SerializeField] private GameActor _actor;

    private void Update()
    {
        CommandInstance cmdInst = InputHandler();
        if (cmdInst != null) cmdInst.Excute(_actor);
    }

    CommandInstance InputHandler()
    {
        if (_playerInput.IsLClick)
            return _button0;

        return null;
    }
}