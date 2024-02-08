using System;
using System.Collections;
using Cysharp.Threading.Tasks;

public class Ga_Heal : AbilityBase
{
    public Ga_Heal(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        abilityName = "Ga_Heal";
    }
    
    
    protected override async UniTask AbilityTask(TargetData targetData)
    {
        await Heal();
    }

    private async UniTask Heal()
    {
        abilitySystem.TryApplyModifier(ModifierPool.Instance.CreateModifier("Mo_Increase_Hp", owner, owner));
        await UniTask.Yield();
    }
}