using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  顶层系统
/// </summary>
public class TopSystem : MonoBehaviour
{
    [SerializeField] private int _gridwidth = 10;
    [SerializeField] private int _gridheight = 10;
    [SerializeField] private float _cellsize = 10f;
    [SerializeField] private Vector3 _originPos = Vector3.zero;

    private CommandCenter _commandCenter;
    private ActorsManagerCenter _actorsManagerCenter;
    private MessageCenter _messageCenter;
    private MapSystem _mapSystem;
    private PlayerInput _playerInput;
    // Test
    private CommandInstance _button0;
    [SerializeField] private GameActor _actor;

    #region #System Functions

    /// <summary>
    /// 初始化系统
    /// </summary>
    private void Awake()
    {
        InitCenter();
        InitInput();
        InitMap();

        _button0 = new CommandMove();
    }

   

    private void Update()
    {
        CommandInstance cmdInst = InputHandler();
        if (cmdInst != null) cmdInst.Excute(_actor);
    }

    #endregion

    CommandInstance InputHandler()
    {
        if (_playerInput.IsLClick)
            return _button0;

        return null;
    }

    #region #Init Functions

    void InitCenter()
    {
        _commandCenter = new CommandCenter();
        _actorsManagerCenter = new ActorsManagerCenter();
        _messageCenter = new MessageCenter();
    }

    void InitInput()
    {
        _playerInput = PlayerInput.Instance;
        _playerInput.EnableGamePlayInputs();
    }

    void InitMap()
    {
        _mapSystem = new MapSystem(_gridwidth, _gridheight, _cellsize, _originPos);
    }

    #endregion
}