using System;
using System.Collections;

public class Ga_Heal : AbilityBase
{
    public Ga_Heal(GameActor owner) : base(owner)
    {
        name = "Ga_Heal";
    }


    public override void OnActive()
    {
        abilitySystem.TryApplyModifier(ModifierPool.Instance.CreateModifier("Mo_Increase_Hp", owner, owner));
    }

    public override void OnFinished()
    {
        
    }

    public override void ActiveAbility(Action onAbilityActive, Action onAbilityEnd)
    {
        onActive?.Invoke();
        onAbilityActive?.Invoke();;
        onFinished?.Invoke();
        onAbilityEnd?.Invoke();
    }
}