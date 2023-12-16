using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem
{
    public Dictionary<string, Modifier> modifierDictionary = new Dictionary<string, Modifier>();
    public Dictionary<string, AbilityBase> abilityDictionary = new Dictionary<string, AbilityBase>();
    public CharacterAttributeSet characterAttributeSet = new CharacterAttributeSet();

    // 回合相关
    public Action onTurnStart;
    public Action onTurnEnd;
    public Action onLifeTimeEnd;

    private Character owner;

    public AbilitySystem(Character owner)
    {
        this.owner = owner;
    }

    public bool CanActiveAbility(string name)
    {
        return abilityDictionary.ContainsKey(name);
    }

    public void TryApplyModifier(Modifier modifier)
    {
        Debug.Log("TryApplyModifier!");
        if (modifier == null) return;
        
        modifierDictionary[modifier.name] = modifier;

        // 回合相关
        onTurnStart += modifier.onTurnStart;
        onTurnEnd += modifier.onTurnEnd;
        onLifeTimeEnd += modifier.onLifeTimeEnd;
        
        // 触发
        modifier.onCreate?.Invoke();
    }

    public void TryRemoveModifier(Modifier modifier)
    {
        Debug.Log("TryRemoveModifier!");
        if (modifierDictionary.Remove(modifier.name))
        {
            onTurnStart -= modifier.onTurnStart;
            onTurnEnd -= modifier.onTurnEnd;
        }
    }

    public void TryApplyAbility(AbilityBase ability)
    {
        if (!abilityDictionary.ContainsKey(ability.name))
            abilityDictionary.Add(ability.name, ability);
    }

    public bool TryActiveAbility(string abilityName)
    {
        Debug.Log("Try Active Ability!");
        if (!abilityDictionary.ContainsKey(abilityName))
            return false;

        abilityDictionary[abilityName].ActiveAbility();
        return true;
    }
}