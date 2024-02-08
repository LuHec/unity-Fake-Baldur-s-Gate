using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem
{
    public Dictionary<string, Modifier> modifierDictionary = new Dictionary<string, Modifier>();
    public Dictionary<string, AbilityBase> abilityDictionary = new Dictionary<string, AbilityBase>();
    public CharacterAttributeSet characterAttributeSet = new CharacterAttributeSet();
    public AbilityTasksProcessor abilityTasksProcessor = new AbilityTasksProcessor();
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
        if (abilityDictionary.ContainsKey(ability.abilityName)) return;

        // 监听技能的开始和结束
        abilityDictionary.Add(ability.abilityName, ability);
        ability.onAbilityActive += onAbilityActive;
        ability.onAbilityFinished += onAbilityEnd;
    }

    public void TryRemoveAbility(String name)
    {
        if (!abilityDictionary.ContainsKey(name)) return;

        var ability = abilityDictionary[name];
        ability.onAbilityActive += onAbilityActive;
        ability.onAbilityFinished += onAbilityEnd;
        abilityDictionary.Remove(name);
    }

    /// <summary>
    /// 自由模式下直接执行任务
    /// </summary>
    /// <param name="abilityName"></param>
    /// <returns></returns>
    [Obsolete("替换为绑定输入版本")]
    public bool TryActiveAbility(string abilityName)
    {
        if (!abilityDictionary.ContainsKey(abilityName))
            return false;

        abilityDictionary[abilityName].ActiveAbility();
        onAbilityActive?.Invoke();
        return true;
    }


    /// <summary>
    /// 单次绑定技能输入，结束后会自动解除绑定
    /// </summary>
    /// <param name="abilityName">激活技能的名字</param>
    /// <param name="confirmHandler">确认</param>
    /// <param name="cancelHandler">取消</param>
    /// <param name="positionHandler">移动</param>
    /// <returns></returns>
    public bool TryActiveAbility(string abilityName,
        ref EventHandler<EventArgsType.PlayerConfirmMessage> confirmHandler,
        ref EventHandler<EventArgsType.PlayerCancelMessage> cancelHandler, ref EventHandler<Vector3> positionHandler)
    {
        // 检测前置条件
        if (!abilityDictionary.ContainsKey(abilityName))
            return false;

        var ability = abilityDictionary[abilityName];
        ability.AbilityBindInput(ref confirmHandler, ref cancelHandler, ref positionHandler);
        ability.ActiveAbility();

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

    public void CommitAbilityTask(AbilityBase ability, string taskName)
    {
        abilityTasksProcessor.CommitTask(ability, taskName);
    }
}