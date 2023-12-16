using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CharacterAttributeSet : GameAttribute
{
    #region Base Attribute

    private float maxHp;
    private float currentHp;

    private int maxSkillPoint;
    private int currentSillPoint;

    private int maxActionPoint;
    private int currentActionPoint;
    
    public float MaxHp
    {
        get => maxHp;
        set 
        { maxHp = value;
            onMaxHpChange();
        }
    }

    public void ModifierCurrentHp(float value)
    {
        currentHp = Mathf.Clamp(currentHp + value, 0, maxHp);
        onCurrentHpChange?.Invoke();
    }

    public int MaxSkillPoint
    {
        get => maxSkillPoint;
        set => maxSkillPoint = value;
    }

    public int CurrentSillPoint
    {
        get => currentSillPoint;
        set => currentSillPoint = value;
    }

    public int MaxActionPoint
    {
        get => maxActionPoint;
        set => maxActionPoint = value;
    }

    public int CurrentActionPoint
    {
        get => currentActionPoint;
        set => currentActionPoint = value;
    }

    #endregion

    #region Delegate

    public Action onCurrentHpChange;
    public Action onMaxHpChange;

    #endregion
}