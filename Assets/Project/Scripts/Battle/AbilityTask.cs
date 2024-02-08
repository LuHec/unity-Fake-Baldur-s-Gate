public class AbilityTask
{
    public string taskName;
    public AbilityBase abilityInstance;

    public AbilityTask( AbilityBase abilityInstance, string taskName)
    {
        this.taskName = taskName;
        this.abilityInstance = abilityInstance;
    }
}