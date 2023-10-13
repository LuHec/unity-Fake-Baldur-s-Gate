using CodeMonkey.Utils;
using UnityEngine;

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
        if (_playerInput.IsLClick) return MoveUnitCommand();
        return null;
    }

    CommandInstance MoveUnitCommand()
    {
        // Debug.Log("Map is" + (_mapSystem == null));
        Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
        
        return new MoveActorCommand(mousePos.x, mousePos.z);
    }
}