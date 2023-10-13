using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnInstance
{
    public GameTurn.Turn NowTurn => _turn;
    private bool _playerControlledTurn;
    private GameTurn.Turn _turn = GameTurn.Turn.PlayerTurn;
    private List<GameActor> _playerControlledActors;
    private List<GameActor> _systemControlledActors;
    private int _playerControlledPtr = 0;
    private CommandCenter _commandCenter;

    public TurnInstance(CommandCenter commandCenter, List<GameActor> playerControlledActors, List<GameActor> systemControlledActors)
    {
        _commandCenter = commandCenter;
        _playerControlledActors = playerControlledActors;
        _systemControlledActors = systemControlledActors;
    }

    public void NextTurn()
    {
        _playerControlledPtr = (_playerControlledPtr + 1) % _playerControlledActors.Count;
    }

    public void RunTurn(Action onExcuteFinished, Action onExcuteError)
    {
        if (NowTurn == GameTurn.Turn.PlayerTurn)
        {
            if (_playerControlledActors != null)
            {
                GameActor actor = _playerControlledActors[_playerControlledPtr];
                _commandCenter.AddCommand(_commandCenter.GetCommandCache(), actor);
                
                // 如果操作非法，将会中断执行并且返回等待命令
                if (_commandCenter.Excute(actor.GetCommand(), actor, onExcuteFinished) == false)
                {
                    onExcuteError();
                }
            }
        }
    }
}