using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputCommandsGenerator
{
    private GameActor actor;
    private PlayerInput playerInput;
    private MapSystem mapSystem;
    private CommandInstance commandCache;
    public bool CanGenCommandCache => commandCache == null;

    public InputCommandsGenerator()
    {
        mapSystem = MapSystem.Instance;
        playerInput = PlayerInput.Instance;
    }

    public InputCommandsGenerator(GameActor actor)
    {
        mapSystem = MapSystem.Instance;
        playerInput = PlayerInput.Instance;
        this.actor = actor;
    }

    /// <summary>
    /// 返回当前监听到的输入指令
    /// </summary>
    /// <returns>CommandInstance</returns>
    private void GenInputCommand()
    {
        if (playerInput.IsLClick)
        {
            // 判断是否点击到UI上，这里的GameObject是EventGameObject
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                commandCache = null;
                return;
            }

            // 检测目标格子是否有人
            Vector3 mousePos = playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));
            // _mapSystem.GetGrid().GetXZ(mousePos.x, mousePos.z, out int xMouse, out int zMouse);
            GameActor targetGridActor = mapSystem.GetGridObject(mousePos.x, mousePos.z).GetActor();

            if (targetGridActor == null)
            {
                commandCache = GetMoveActorCommand();
            }
            else
            {
                commandCache = GetAttackActorCommand(targetGridActor);
            }
        }
    }

    public CommandInstance GetCommand()
    {
        // 运行Cache状态下不能产生新的命令
        // 闲置状态下会返回空
        if (CanGenCommandCache)
        {
            GenInputCommand();
        }

        return commandCache;
    }

    CommandInstance GetMoveActorCommand()
    {
        // Debug.Log("Map is" + (_mapSystem == null));
        Vector3 mousePos = playerInput.GetMouse3DPosition(LayerMask.GetMask("Default"));

        return new MoveActorCommand(mousePos.x, mousePos.z, actor.transform.position);
    }

    CommandInstance GetAttackActorCommand(GameActor gridActor)
    {
        return new AttackActorCommand(gridActor);
    }

    public void ClearCommandCache()
    {
        commandCache = null;
    }
}