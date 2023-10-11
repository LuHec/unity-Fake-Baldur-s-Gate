using UnityEngine;
public class InputCommandsGenerator
{
    private PlayerInput _playerInput;
    private MapSystem _mapSystem;

    public InputCommandsGenerator(MapSystem mapSystem)
    {
        _mapSystem = mapSystem;
        _playerInput = PlayerInput.Instance;
        _playerInput.EnableGamePlayInputs();
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
        Vector3 mousePos = _playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
        return new MoveActorCommand(_mapSystem, mousePos.x, mousePos.z);
    }
}