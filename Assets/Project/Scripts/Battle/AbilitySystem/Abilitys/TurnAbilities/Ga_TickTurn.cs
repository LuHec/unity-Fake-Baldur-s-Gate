using Cysharp.Threading.Tasks;

public class Ga_TickTurn : AbilityBase
{
    public Ga_TickTurn(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        abilityName = "Ga_TickTurn";
    }

    protected override async UniTask AbilityTask(TargetData targetData)
    {
        await UniTask.WaitForSeconds(1);
    }
}