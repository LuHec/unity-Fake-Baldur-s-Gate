public class Ga_Heal : AbilityBase
{
    public Ga_Heal(GameActor owner) : base(owner)
    {
        name = "Ga_Heal";
    }


    public override void OnCreate()
    {
        abilitySystem.TryApplyModifier(ModifierPool.Instance.CreateModifier("Mo_Decrease_Hp", owner, owner));
    }

    public override void OnFinished()
    {
        
    }
}