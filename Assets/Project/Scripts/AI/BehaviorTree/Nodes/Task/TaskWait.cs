public class TaskWait : BehaviorNode
{
    private Character _character;

    public TaskWait(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        WaitActorCommand waitActorCommand = new WaitActorCommand();
        return NodeState.SUCCESS;
    }
}