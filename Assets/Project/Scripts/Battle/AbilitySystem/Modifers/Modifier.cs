using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 命名规则: MO_XXX
/// </summary>
/// 
public class Modifier
{
    #region Attribute

    public string name;
    public int maxTurn;
    public int currentTurn;

    public GameActor owner;
    public GameActor target;

    #endregion

    #region Modifier Event

    public Action onCreate;
    public Action onTurnStart;
    public Action onTurnEnd;
    public Action onOwnerDamaged;
    public Action onTargetKilled;
    public Action onLifeTimeEnd;

    #endregion

    #region Operation

    public static void DecreaseHp(GameActor actor, float hp)
    {
        Debug.Log("DecreaseHp!");
        Character character = (Character)actor;
        character.abilitySystem.characterAttributeSet.ModifyCurrentHp(-hp);
    }

    public static void IncreaseHp(GameActor actor, float hp)
    {
        Debug.Log("IncreaseHp!");
        Character character = (Character)actor;
        character.abilitySystem.characterAttributeSet.ModifyCurrentHp(hp);
    }

    #endregion

    #region Data Dictionary

    private static readonly Dictionary<Operation, Action<GameActor, float>> OperationDict =
        new Dictionary<Operation, Action<GameActor, float>>()
        {
            { Operation.DECREASE_HP, DecreaseHp },
            { Operation.INCREASE_HP, IncreaseHp },
        };

    #endregion

    public Modifier(ModifierCfgBase cfg, GameActor owner, GameActor target)
    {
        Init(cfg, owner, target);
    }

    public void Init(ModifierCfgBase cfg, GameActor owner, GameActor target)
    {
        // 自动解析
        name = cfg.name;
        this.owner = owner;
        this.target = target;

        // 根据持续时间订阅
        switch (cfg.duration)
        {
            // 立即触发
            case Duration.INSTANT:
                maxTurn = 1;
                currentTurn = maxTurn;

                onTurnEnd += DecreaseTurn;
                onLifeTimeEnd += EndLife;
                break;
            // 持续多个回合非立即触发
            case Duration.MULTI_TURN:
                maxTurn = cfg.maxTurn;
                currentTurn = maxTurn;

                onTurnEnd += DecreaseTurn;
                onLifeTimeEnd += EndLife;
                break;
            // 永久持续，什么都不做
            case Duration.INFINITY:
                break;
            default:
                break;
        }

        // 事件配置
        foreach (var modifierOperation in cfg.onCreate)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onCreate += () => { OperationDict[modifierOperation.operation](target, modifierOperation.value); };
        }

        foreach (var modifierOperation in cfg.onTurnStart)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onTurnStart += () => { OperationDict[modifierOperation.operation](target, modifierOperation.value); };
        }

        foreach (var modifierOperation in cfg.onTurnEnd)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onTurnEnd += () => { OperationDict[modifierOperation.operation](target, modifierOperation.value); };
        }

        foreach (var modifierOperation in cfg.onOwnerDamaged)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onOwnerDamaged += () => { OperationDict[modifierOperation.operation](owner, modifierOperation.value); };
        }

        foreach (var modifierOperation in cfg.onTargetKilled)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onTargetKilled += () =>
                {
                    OperationDict[modifierOperation.operation](target, modifierOperation.value);
                };
        }

        foreach (var modifierOperation in cfg.onLifeTimeEnd)
        {
            if (OperationDict.ContainsKey(modifierOperation.operation))
                onLifeTimeEnd += () => { OperationDict[modifierOperation.operation](target, modifierOperation.value); };
        }
    }

    /// <summary>
    /// 每回合检查
    /// </summary>
    public void TurnCheck()
    {
        if (currentTurn == 0)
            onLifeTimeEnd?.Invoke();
    }

    /// <summary>
    /// 每回合结束都要衰减
    /// </summary>
    public void DecreaseTurn()
    {
        currentTurn--;
        TurnCheck();
    }

    /// <summary>
    /// 回收buff，取消相关的订阅
    /// </summary>
    public void EndLife()
    {
        Debug.Log("Modifier end!");
        // 清除ASC的相关事件
        var abilitySystem = ((Character)owner).abilitySystem;
        abilitySystem.TryRemoveModifier(this);
    }
}