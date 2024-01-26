using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CharacterAttributeSet : GameAttribute
{
    #region Base Attribute
    // ------------------------------------------------------------------------------
    // properties with delegates
    private float maxHp = 100;
    private float currentHp = 100;
    
    private float maxSpeed = 5;
    private float currentSpeed = 5;

    // ------------------------------------------------------------------------------
    // properties without delegates
    private bool bDeath = false;
    
    private int speedATB = 1;

    private int maxActionPoint = 10;
    private int currentActionPoint = 10;

    private int maxSkillPoint = 3;
    private int currentSillPoint = 3;
    
    // ------------------------------------------------------------------------------
    // properties getter
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public bool BDeath => bDeath;
    public int MaxActionPoint => maxActionPoint;
    public int CurrentActionPoint => currentActionPoint;
    public int MaxSkillPoint => maxSkillPoint;
    public int CurrentSillPoint => currentSillPoint;
    public float MaxSpeed => maxSpeed;
    public float CurrentSpeed => currentSpeed;
    public int SpeedAtb => speedATB;
   

    // ------------------------------------------------------------------------------
    // properties modify
    public void ModifyMaxHp(float hp)
    {
        // 限制血量大小
        maxHp = Mathf.Clamp(maxHp + hp, 0, maxHp + hp);
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (hp > 0)
        {
            onMaxHpIncrease?.Invoke();
        }
        else if (hp < 0)
        {
            onMaxHpDecrease?.Invoke();
        }
    }

    public void ModifyCurrentHp(float hp)
    {
        currentHp = Mathf.Clamp(currentHp + hp, 0, maxHp);

        if (hp > 0)
        {
            onCurrentHpIncrease?.Invoke();
        }
        else if (hp < 0)
        {
            onCurrentHpDecrease?.Invoke();
        }
    }

    public void ModifierDeathState(bool death)
    {
        bDeath = death;
    }

    public void ModifyCurrentSpeed(float speed)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + speed, 0, maxSpeed);
    }

    public void ModifyMaxSpeed(float speed)
    {
        maxSpeed = Mathf.Clamp(maxSpeed + speed, 0, maxSpeed + speed);
    }


    #endregion

    #region Delegate

    public Action onCurrentHpIncrease;
    public Action onCurrentHpDecrease;
    public Action onMaxHpIncrease;
    public Action onMaxHpDecrease;

    #endregion
}