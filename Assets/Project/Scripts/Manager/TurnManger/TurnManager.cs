using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

/// <summary>
/// 回合管理器
/// </summary>
public class TurnManager
{
    private List<TurnInstance> _turnInstances;
    private State _state = State.WaitCommand;
    private CommandCenter _commandCenter;
    private MessageCenter _messageCenter;

    enum State
    {
        WaitCommand,
        RunCommand,
    }

    public TurnManager(CommandCenter commandCenter)
    {
        _commandCenter = commandCenter;
        _messageCenter = MessageCenter.Instance;
        _turnInstances = new List<TurnInstance>();
    }

    public void AddTurn(List<GameActor> playerCon, List<GameActor> systemCon)
    {
        _turnInstances.Add(new TurnInstance(_commandCenter, playerCon, systemCon));
    }

    public void RunTurn()
    {
        if (_messageCenter.globalState.TurnMode)
        {
            RunTurnMode();
        }
        else
        {
            Run3RdMode();
        }
    }

    public void RunTurnMode()
    {
        switch (_state)
        {
            case State.WaitCommand:
            {
                if (_commandCenter.GenInputCommandCache())
                {
                    _state = State.RunCommand;
                    _turnInstances[0].RunTurn(
                        () =>
                        {
                            _state = State.WaitCommand;
                            _turnInstances[0].NextTurn();
                        },
                        () => { _state = State.WaitCommand; }
                    );
                }

                break;
            }
            case State.RunCommand:
            {
                _turnInstances[0].RunTurn(
                    () =>
                    {
                        _state = State.WaitCommand;
                        _turnInstances[0].NextTurn();
                    },
                    () => { _state = State.WaitCommand; }
                );
                break;
            }
        }
    }

    public void Run3RdMode()
    {
        _commandCenter.GenInputCommandCache();

        _turnInstances[0].RunTurn(
            () => { },
            () => { }
        );
    }

    void OnExcuteFinished()
    {
        _state = State.WaitCommand;
        _turnInstances[0].NextTurn();
    }

    void OnExcuteError()
    {
        _state = State.WaitCommand;
    }
}

public class GameTurn
{
    public enum Turn
    {
        PlayerTurn,
        SystemTurn
    }
}