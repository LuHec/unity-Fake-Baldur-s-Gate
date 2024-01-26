/// <summary>
/// 检测是否可以生成命令
/// </summary>
public class CheckCommandCache : BehaviorNode
{
    private Character _character;
    
    public CheckCommandCache(Character character)
    {
        _character = character;
    }


    public override NodeState Evaluate()
    {
        // if (_character.IsCommandCacheEmpty())
            return NodeState.SUCCESS;
        
        return NodeState.FAILURE;
    }
}