using System;
using System.Collections;

/// <summary>
/// 命名方式：Ga_XXX 
/// </summary>
public abstract class AbilityBase
{
    public float timer;
    public string name;
    public GameActor owner;
    public AbilitySystem abilitySystem;

    public Action onCreate;
    public Action onFinished;
    public bool isRunning = true;

    protected AbilityBase(GameActor owner)
    {
        this.owner = owner;

        var character = (Character)owner;
        abilitySystem = character.abilitySystem;

        onCreate += OnCreate;
        onFinished += OnFinished;
        onFinished += () => { isRunning = false; };
    }

    public abstract void OnCreate();
    public abstract void OnFinished();

    public void ActiveAbility()
    {
        onCreate?.Invoke();
    }

    public void CommitTask(IEnumerator task)
    {
        TurnManager.Instance.StartCoroutine(task);
    }
}