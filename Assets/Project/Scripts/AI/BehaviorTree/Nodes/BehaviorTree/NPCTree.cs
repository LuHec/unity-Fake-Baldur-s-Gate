using System.Collections.Generic;

public class NPCTree : BehaviorTree
{
    public override void RunUpdate()
    {
        // GenCommand
        if (root != null)
            root.Evaluate();
    }

    protected override BehaviorNode SetUpTree()
    {
        root = new Selector(
                new List<BehaviorNode>()
                {
                    
                }
            );
        
        return root;
    }
}