public class CheckAttackRange : BehaviorNode
{
    private Character _character;

    public CheckAttackRange(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        return NodeState.SUCCESS;
    }
}