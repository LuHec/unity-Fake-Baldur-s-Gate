using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}

public class BehaviorNode
{
    public BehaviorNode Parent => parent;
    private BehaviorNode parent;

    protected Character character;
    protected NodeState state;
    protected List<BehaviorNode> childNodes;
    protected Dictionary<string, object> dataBase;

    #region #Init

    public BehaviorNode()
    {
        parent = null;
    }

    public BehaviorNode(List<BehaviorNode> childNodes)
    {
        this.childNodes = new List<BehaviorNode>();
        foreach (var child in childNodes)
        {
            Attach(child);
        }
    }

    private void Attach(BehaviorNode child)
    {
        child.parent = this;
        childNodes.Add(child);
    }

    #endregion

    #region #Run

    public virtual NodeState Evaluate()
    {
        return NodeState.FAILURE;
    }

    void SetData(string key, object value)
    {
        dataBase[key] = value;
    }

    object GetData(string key)
    {
        object obj = null;
        if (dataBase.ContainsKey(key))
            return dataBase[key];

        BehaviorNode behaviorNode = parent;
        while (behaviorNode != null)
        {
            var value = behaviorNode.GetData(key);
            if (value != null)
                return value;

            behaviorNode = behaviorNode.parent;
        }

        return null;
    }

    bool ClearData(string key)
    {
        object obj = null;
        if (dataBase.ContainsKey(key))
        {
            dataBase.Remove(key);
            return true;
        }

        BehaviorNode behaviorNode = parent;
        while (behaviorNode != null)
        {
            if (ClearData(key))
                return true;

            behaviorNode = behaviorNode.parent;
        }

        return false;
    }

    #endregion
}