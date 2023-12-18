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

    public override void ActiveAbility(Action onAbilityEnd = null)
    {
        onActive?.Invoke();
        onFinished?.Invoke();
        onAbilityEnd?.Invoke();
    }
}