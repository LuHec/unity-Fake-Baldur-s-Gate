using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem
{
    public Dictionary<string, Modifier> modifierDictionary = new Dictionary<string, Modifier>();
    public Dictionary<string, AbilityBase> abilityDictionary = new Dictionary<string, AbilityBase>();
    public CharacterAttributeSet characterAttributeSet = new CharacterAttributeSet();
    public Character Owner => owner;
    private Character owner;

    // 回合相关
    public struct AbilityDelegate
    {
        public Action onTurnStart;
        public Action onActEnd;
        public Action onTurnEnd;
        public Action onTurn;
        public Action onAbilityEnd;
    }

    public AbilityDelegate abilityDelegate = new AbilityDelegate();


    public Action onTurnStart;
    public Action onTurnEnd;
    public Action onLifeTimeEnd;
    public Action onAbilityActive;
    public Action onAbilityEnd;

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

    /// <summary>
    /// 会交给Modifer自己清除
    /// </summary>
    /// <param name="modifier"></param>
    public void TryRemoveModifier(Modifier modifier)
    {
        Debug.Log("TryRemoveModifier!");
        if (modifierDictionary.Remove(modifier.name))
        {
            onTurnStart -= modifier.onTurnStart;
            onTurnEnd -= modifier.onTurnEnd;
            onLifeTimeEnd -= modifier.onLifeTimeEnd;
        }
    }

    public void TryApplyAbility(AbilityBase ability)
    {
        if(abilityDictionary.ContainsKey(ability.name)) return;
        
        // 监听技能的开始和结束
        abilityDictionary.Add(ability.name, ability);
        ability.onActive += onAbilityActive;
        ability.onActive += onAbilityEnd;
    }

    public void TryRemoveAbility(String name)
    {
        if(!abilityDictionary.ContainsKey(name)) return;

        var ability = abilityDictionary[name];
        ability.onActive += onAbilityActive;
        ability.onActive += onAbilityEnd;
        abilityDictionary.Remove(name);
    }

    /// <summary>
    /// 自由模式下直接执行任务
    /// </summary>
    /// <param name="abilityName"></param>
    /// <returns></returns>
    public bool TryActiveAbility(string abilityName)
    {
        if (!abilityDictionary.ContainsKey(abilityName))
            return false;
        
        abilityDictionary[abilityName].ActiveAbility();
        onAbilityActive?.Invoke();
        return true;
    }

    /// <summary>
    /// 回合模式下返回任务
    /// </summary>
    /// <param name="abilityName"></param>
    /// <returns></returns>
    public AbilityBase TryGetAbility(string abilityName)
    {
        if (!abilityDictionary.ContainsKey(abilityName))
            return null;
        
        return abilityDictionary[abilityName];
    }
}