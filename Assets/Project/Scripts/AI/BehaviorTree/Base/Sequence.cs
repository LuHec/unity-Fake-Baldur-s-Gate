using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 会执行所有的Task/Sequence，一旦失败就结束
/// </summary>
public class Sequence : BehaviorNode
{
    public Sequence() : base() { }
    public Sequence(List<BehaviorNode> children) : base(children) { }
    
    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;
        foreach (var child in childNodes)
        {
            switch (child.Evaluate())
            {
                // 成功会继续执行，只要有一个失败就会返回
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildIsRunning = true;
                    continue;

                default:
                    state = NodeState.SUCCESS;
                    return state;
            }
        }

        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
