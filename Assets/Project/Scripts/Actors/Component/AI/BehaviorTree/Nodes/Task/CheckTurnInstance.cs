/// <summary>
/// 检测是否在回合内
/// </summary>
public class CheckTurnInstance : BehaviorNode
{
    private Character _character;
    
    public CheckTurnInstance(Character character)
    {
        _character = character;
    }


    public override NodeState Evaluate()
    {
        if (_character.CurrentTurn != null)
            return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }
}