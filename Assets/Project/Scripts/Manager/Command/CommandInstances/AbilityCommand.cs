using System;

public class AbilityCommand : CommandInstance
{
    private GameActor owner;
    private string abilityName;
    private AbilitySystem abilitySystem;

    public AbilityCommand(GameActor actor, string abilityName)
    {
        owner = actor;
        this.abilityName = abilityName;
        abilitySystem = ((Character)actor).abilitySystem;
    }

    public override bool Excute(GameActor actor, Action onExcuteFinsihed)
    {
        hasExecuted = true;

        abilitySystem.TryActiveAbility(abilityName, () =>
        {
            isRunning = false;
            onExcuteFinsihed?.Invoke();
        });
        return true;
    }

    public override void Undo(GameActor actor)
    {
    }
}