using Cysharp.Threading.Tasks;

public class Ga_TickTurn : AbilityBase
{
    public Ga_TickTurn(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        name = "Ga_TickTurn";
    }

    protected override async UniTask AbilityTask()
    {
        await UniTask.WaitForSeconds(1);
    }
}