using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputCommandsGenerator
{
    private PlayerInput _playerInput;
    private MapSystem _mapSystem;

    public InputCommandsGenerator()
    {
        _mapSystem = MapSystem.Instance;
        _playerInput = PlayerInput.Instance;
    }

    /// <summary>
    /// 返回当前监听到的输入指令
    /// </summary>
    /// <returns>CommandInstance</returns>
    public CommandInstance GetInputCommand()
    {
        if (_playerInput.IsLClick)
        {
            // 处理UI部分
            if (EventSystem.current.IsPointerOverGameObject()) return null;

            // 检测目标格子是否有人
            Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
            // _mapSystem.GetGrid().GetXZ(mousePos.x, mousePos.z, out int xMouse, out int zMouse);
            GameActor targetGridActor = _mapSystem.GetGridObject(mousePos.x, mousePos.z).GetActor();
            
            if (targetGridActor == null)
            {
                return GetMoveActorCommand();
            }
            else
            {
                return GetAttackActorCommand(targetGridActor);
            }
        }

        return null;
    }

    CommandInstance GetMoveActorCommand()
    {
        // Debug.Log("Map is" + (_mapSystem == null));
        Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));

        return new MoveActorCommand(mousePos.x, mousePos.z);
    }

    CommandInstance GetAttackActorCommand(GameActor gridActor)
    {
        return new AttackActorCommand(gridActor);
    }
}