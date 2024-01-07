using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 只会执行第一个成功的Sequence/Task
/// </summary>
public class Selector : BehaviorNode
{
    public Selector() : base() { }
    public Selector(List<BehaviorNode> children) : base(children) { }

    public override NodeState Evaluate()
    {
        foreach (var child in childNodes)
        {
            switch (child.Evaluate())
            {
                // 失败了选择下一个序列
                case NodeState.FAILURE:
                    continue;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;

                default:
                    continue;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}