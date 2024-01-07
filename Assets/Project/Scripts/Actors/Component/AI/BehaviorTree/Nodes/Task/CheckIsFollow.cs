/// <summary>
/// 检测是否为随从
/// </summary>
public class CheckIsFollow : BehaviorNode
{
    private Character _character;

    public CheckIsFollow(Character character)
    {
        _character = character;
    }

    public override NodeState Evaluate()
    {
        if (_character.GetCharacterType() == ActorEnumType.AIMode.Follow)
            return NodeState.SUCCESS;
        return NodeState.FAILURE;
    }
}