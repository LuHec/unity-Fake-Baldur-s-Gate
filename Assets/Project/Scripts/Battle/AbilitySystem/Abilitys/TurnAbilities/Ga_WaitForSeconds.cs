using Cysharp.Threading.Tasks;

public class Ga_WaitForSeconds : AbilityBase
{
    public Ga_WaitForSeconds(AbilitySystem abilitySystem) : base(abilitySystem)
    {
        
    }

    protected override async UniTask AbilityTask(TargetData targetData)
    {
        await UniTask.WaitForSeconds(1);
    }
}