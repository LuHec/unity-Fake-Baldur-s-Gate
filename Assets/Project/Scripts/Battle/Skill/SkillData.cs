using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Skill Type")] 
    [Tooltip("技能持续时间")]
    public SkillTurnType skillTurnType;
    [Tooltip("攻击对象(单个/多个)")]
    public AttackType attackType;
    [Tooltip("攻击范围")]
    public SkillAreaType skillAreaType;


    [Space] [Header("Skill Value")] 
    [Tooltip("伤害值")]
    public float damage = 0;
    [Tooltip("行动点消耗")]
    public int cost = 10;
    [Tooltip("技能有效释放范围")]
    public float distance = 10f;

    [Space] [Header("Description")] 
    public string description;
}

public enum SkillTurnType
{
    SINGLE_TURN,
    MULTI_TURN
}

public enum AttackType
{
    SINGLE,
    AOE,
}

public enum SkillAreaType
{
    DIRECTED,
    SECTOR,
    CIRCULAR
}