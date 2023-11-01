using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputCommandsGenerator
{
    private GameActor _actor;
    private PlayerInput _playerInput;
    private MapSystem _mapSystem;
    private CommandInstance _commandCache;
    public bool CanGenCommandCache => _commandCache == null;

    public InputCommandsGenerator()
    {
        _mapSystem = MapSystem.Instance;
        _playerInput = PlayerInput.Instance;
    }

    public InputCommandsGenerator(GameActor actor)
    {
        _mapSystem = MapSystem.Instance;
        _playerInput = PlayerInput.Instance;
        _actor = actor;
    }

    /// <summary>
    /// 返回当前监听到的输入指令
    /// </summary>
    /// <returns>CommandInstance</returns>
    private void GenInputCommand()
    {
        if (_playerInput.IsLClick)
        {
            // 判断是否点击到UI上，这里的GameObject是EventGameObject
            if (EventSystem.current.IsPointerOverGameObject())
            {
                _commandCache = null;
                return;
            }

            // 检测目标格子是否有人
            Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
            // _mapSystem.GetGrid().GetXZ(mousePos.x, mousePos.z, out int xMouse, out int zMouse);
            GameActor targetGridActor = _mapSystem.GetGridObject(mousePos.x, mousePos.z).GetActor();

            if (targetGridActor == null)
            {
                _commandCache = GetMoveActorCommand();
            }
            else
            {
                _commandCache = GetAttackActorCommand(targetGridActor);
            }
        }
    }

    public CommandInstance GetCommand()
    {
        // 运行Cache状态下不能产生新的命令
        if (CanGenCommandCache)
        {
            GenInputCommand();
        }

        return _commandCache;
    }

    CommandInstance GetMoveActorCommand()
    {
        // Debug.Log("Map is" + (_mapSystem == null));
        Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));

        return new MoveActorCommand(mousePos.x, mousePos.z, _actor.transform.position);
    }

    CommandInstance GetAttackActorCommand(GameActor gridActor)
    {
        return new AttackActorCommand(gridActor);
    }

    public void ClearCommandCache()
    {
        _commandCache = null;
    }
}