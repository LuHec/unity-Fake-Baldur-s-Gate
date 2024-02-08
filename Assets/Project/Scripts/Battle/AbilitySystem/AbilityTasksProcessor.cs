using System;
using System.Collections.Generic;

public class AbilityTasksProcessor
{
    public enum ProcessorState
    {
        NULL = 0,
        RUNNING,
        IDLE,
        PAUSE,
    }

    public ProcessorState processorState = ProcessorState.NULL;
    private AbilityTask currentAbilityTask;
    private Queue<AbilityTask> abilityTaskQueue = new Queue<AbilityTask>();

    public void CommitTask(AbilityBase ability, string taskName)
    {
        abilityTaskQueue.Enqueue(new AbilityTask(ability, taskName));

        if (processorState == ProcessorState.NULL) processorState = ProcessorState.IDLE;

        TryExecuteTask();
    }

    /// <summary>
    /// 所有活动结尾都需要尝试启动Task
    /// </summary>
    /// <returns></returns>
    public bool TryExecuteTask()
    {
        // 激活后只需要监听结束事件
        if (processorState != ProcessorState.IDLE) return false;

        currentAbilityTask = abilityTaskQueue.Peek();
        abilityTaskQueue.Dequeue();

        // 激活时订阅
        processorState = ProcessorState.RUNNING;
        currentAbilityTask.abilityInstance.onAbilityFinished += OnAbilityTaskEnd;
        currentAbilityTask.abilityInstance.ExecuteAbility();
        return true;
    }

    [Obsolete("暂时用不上")]
    public bool TryCancelTask()
    {
        if (processorState != ProcessorState.RUNNING) return false;

        return true;
    }

    private void OnAbilityTaskEnd()
    {
        // 结束时退订
        currentAbilityTask.abilityInstance.onAbilityFinished -= OnAbilityTaskEnd;
        currentAbilityTask = null;

        processorState = abilityTaskQueue.Count > 0 ? ProcessorState.IDLE : ProcessorState.NULL;

        TryExecuteTask();
    }
}