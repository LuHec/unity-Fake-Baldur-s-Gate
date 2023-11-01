using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorTree
{
    protected BehaviorNode root = null;

    public abstract void RunUpdate();

    protected abstract BehaviorNode SetUpTree();
}