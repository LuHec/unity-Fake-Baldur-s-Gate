using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum Duration
{
    INSTANT,
    MULTI_TURN,
    INFINITY
}


[Serializable]
public struct ModifierOperation
{
    public Operation operation;
    public float value;
}

public enum Operation
{
    NONE,
    DECREASE_HP,
    INCREASE_HP
}


[CreateAssetMenu(menuName = "Data/Modifier/ModifierCfg")]
public class ModifierCfgBase : ScriptableObject
{
    [Header("名称")] public string name;
    [Header("持续时间，只有在选择为MULTI_TURN时有效")] public int maxTurn;
    [Header("持续时间")] public Duration duration;
    [Header("创建时触发")] public ModifierOperation[] onCreate;
    [Header("回合开始时触发")] public ModifierOperation[] onTurnStart;
    [Header("回合结束时触发")] public ModifierOperation[] onTurnEnd;
    [Header("受伤时触发")] public ModifierOperation[] onOwnerDamaged;
    [Header("击杀目标时触发")] public ModifierOperation[] onTargetKilled;
    [Header("销毁时触发")] public ModifierOperation[] onLifeTimeEnd;
    
}