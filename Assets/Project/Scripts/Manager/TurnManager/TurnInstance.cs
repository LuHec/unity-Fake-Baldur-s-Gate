using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurnInstance
{
    // ------------------------------------------------------------------------------
    // Actors Parameters
    public List<TurnItem> turnItemsLists = new List<TurnItem>();
    public Queue<TurnItem> turnItemsQueue = new Queue<TurnItem>();

    // ------------------------------------------------------------------------------
    // Turn Parameters
    private struct TurnDelegate
    {
        public Action onTurnStart;
        public Action onActorEndAct;
        public Action onTurnEnd;
        public Action onStartAction;
        public Action onDestroyTurn;
    }

    private enum TurnState
    {
        TURN_WAIT = 0,
        TURN_BEGIN,
        TURN_BEGIN_ACTION,
        TURN_BEGIN_ACTION_RUNNING,
        TURN_RUNNING,
        TURN_END_ACTION,
        TURN_END_ACTION_RUNNING,
        TURN_END,
    }

    private Queue<AbilityBase> abilityQueue = new Queue<AbilityBase>();
    private TurnDelegate turnDelegate = new TurnDelegate();
    private AbilitySystem abilitySystem;
    private TurnItem tickTurnItem;

    private int turnCounter = 0;
    private TurnState turnState = TurnState.TURN_WAIT;
    private TurnState preTurnState = TurnState.TURN_BEGIN;

    private bool bEndTurn = false;


    // ------------------------------------------------------------------------------
    // Initialize Functions
    public TurnInstance()
    {
        abilitySystem = new AbilitySystem(null);
        tickTurnItem = new TurnItem(this, abilitySystem);

        abilitySystem.TryApplyAbility(new Ga_TickTurn(abilitySystem));
        abilitySystem.TryApplyAbility(new Ga_WaitForSeconds(abilitySystem));

        turnState = TurnState.TURN_WAIT;
        preTurnState = TurnState.TURN_BEGIN;

        turnItemsLists.Add(tickTurnItem);
    }

    public void BeginTurn()
    {
        SortByActorSpeed();
    }

    // ------------------------------------------------------------------------------
    // Run Turn Functions
    public void UpdateTurn()
    {
        // switch (turnState)
        // {
        //         case TurnState.TURN_BEGIN:
        //             // 加入回合事件
        //             actionQueue.Enqueue(WaitOneSecond);
        //
        //             // 角色监听事件
        //             turnDelegate.onTurnStart += characterQueue.Peek().abilitySystem.abilityDelegate.onTurnStart;
        //             turnDelegate.onTurnEnd += characterQueue.Peek().abilitySystem.abilityDelegate.onTurnEnd;
        //             characterQueue.Peek().onEndAction += EndAction;
        //
        //             turnDelegate.onTurnStart?.Invoke();
        //
        //             SwitchState(TurnState.TURN_BEGIN_ACTION);
        //             break;
        //
        //         case TurnState.TURN_BEGIN_ACTION:
        //             ExecuteActions();
        //
        //             SwitchState(TurnState.TURN_BEGIN_ACTION_RUNNING);
        //             break;
        //
        //         case TurnState.TURN_BEGIN_ACTION_RUNNING:
        //             if (actionQueue.Count == 0)
        //             {
        //                 SwitchState(TurnState.TURN_RUNNING);
        //             }
        //
        //             break;
        //
        //         case TurnState.TURN_RUNNING:
        //             // 当ability Queue 没有执行完毕的时候，需要等待执行完毕
        //             // 获取新的命令后立刻执行，如果结束行动，进入异步函数会立刻返回，然后转换到.TURN_END_ACTION状态
        //             if (abilityQueue.Count == 0)
        //             {
        //                 // 由角色决定是否进入下一个回合，TurnInstance负责监听
        //                 characterQueue.Peek().ActorUpdate();
        //                 ExecuteAbilities();
        //             }
        //
        //             break;
        //
        //         case TurnState.TURN_END_ACTION:
        //             ExecuteActions();
        //
        //             SwitchState(TurnState.TURN_END_ACTION_RUNNING);
        //             // turnState = TurnState.TURN_END_ACTION_RUNNING;
        //             break;
        //
        //         case TurnState.TURN_END_ACTION_RUNNING:
        //             if (actionQueue.Count == 0)
        //                 SwitchState(TurnState.TURN_END);
        //             // turnState = TurnState.TURN_END;
        //             break;
        //
        //         case TurnState.TURN_END:
        //             turnCounter++;
        //             // 清除委托
        //             turnDelegate.onTurnStart -= characterQueue.Peek().abilitySystem.abilityDelegate.onTurnStart;
        //             turnDelegate.onTurnEnd -= characterQueue.Peek().abilitySystem.abilityDelegate.onTurnEnd;
        //             characterQueue.Peek().onEndAction -= EndAction;
        //
        //             // 回合结束后进行重排序
        //             SortByActorSpeed();
        //             CheckTurn();
        //
        //             SwitchState(TurnState.TURN_BEGIN);
        //             // turnState = TurnState.TURN_BEGIN;
        //             break;
        // }

        switch (turnState)
        {
            case TurnState.TURN_WAIT:
                // 暂停状态
                break;
            case TurnState.TURN_BEGIN:
                // 暂定，停顿一秒转换
                SwitchState(TurnState.TURN_BEGIN_ACTION);
                abilityQueue.Enqueue(abilitySystem.TryGetAbility("Ga_WaitForSeconds"));
                break;
            case TurnState.TURN_BEGIN_ACTION:
                // 开始执行回合初的任务
                SwitchState(TurnState.TURN_BEGIN_ACTION_RUNNING);
                ExecuteAbilities();
                break;
            case TurnState.TURN_BEGIN_ACTION_RUNNING:
                // 执行任务
                if (abilityQueue.Count == 0) SwitchState(TurnState.TURN_RUNNING);
                break;
            case TurnState.TURN_RUNNING:
                // 运行角色,获取任务,监听角色的回合结束消息
                // 如果添加了任务，则异步进行ExecuteAbilities，不修改状态
                // 如果主动结束了回合，则跳转至下一个状态
                // 但是结束回合调用是异步的，需要等任务执行完毕时才能转换状态，此时不能更新角色的行动
                if (abilityQueue.Count == 0 && bEndTurn == false)
                {
                    turnItemsQueue.Peek().UpdateItem();
                    ExecuteAbilities();
                }

                break;
            case TurnState.TURN_END_ACTION:
                // 开始执行回合结束的任务
                SwitchState(TurnState.TURN_END_ACTION_RUNNING);
                ExecuteAbilities();
                break;
            case TurnState.TURN_END_ACTION_RUNNING:
                // 执行任务
                if (abilityQueue.Count == 0) SwitchState(TurnState.TURN_RUNNING);
                break;
            case TurnState.TURN_END:
                // 回合结束，当前角色ATB清空,增加ATB量表，排序入队,更新ui
                turnItemsQueue.Peek().ClearAtb();
                foreach (var turnItem in turnItemsLists)
                {
                    if (turnItem.abilitySystem.characterAttributeSet.BDeath == false)
                    {
                        turnItem.IncreaseAtb();
                    }
                }

                turnItemsLists.Sort((x, y) =>
                {
                    if (x.currentAtbAmount > y.currentAtbAmount) return 1;
                    else if (x.currentAtbAmount < y.currentAtbAmount) return -1;
                    else return 0;
                });
                turnItemsQueue.Clear();

                foreach (var turnItem in turnItemsLists)
                {
                    if (turnItem.abilitySystem.characterAttributeSet.BDeath == false)
                    {
                        turnItemsQueue.Enqueue(turnItem);
                    }
                }

                UpdateView();

                SwitchState(TurnState.TURN_BEGIN);
                break;
        }
    }

    private void SwitchState(TurnState newState)
    {
        preTurnState = turnState;
        turnState = newState;
    }

    // 仅在角色运行状态下有效
    private async void EndAction()
    {
        if (turnState != TurnState.TURN_RUNNING) return;

        while (abilityQueue.Count > 0)
        {
            await UniTask.Yield();
        }

        turnState = TurnState.TURN_END_ACTION;
    }

    private async void ExecuteAbilities()
    {
        if (abilityQueue.Count == 0) return;

        while (abilityQueue.Count > 0)
        {
            await UniTask.RunOnThreadPool(abilityQueue.Peek().ActiveAbility);
            abilityQueue.Dequeue();
        }
    }

    private async void WaitOneSecond()
    {
        await UniTask.WaitForSeconds(1);
    }

    // ------------------------------------------------------------------------------
    // Update info

    private void SortByActorSpeed()
    {
        turnItemsLists.Sort();
    }

    private void CheckTurn()
    {
        // 只有敌对npc，或只有玩家角色
        int npcCount = 0;
        int playerCount = 0;
        foreach (var turnItem in turnItemsLists)
        {
            if (turnItem.BIsCharacter)
            {
                if (turnItem.character.GetCharacterType() == ActorEnumType.AIMode.Npc) npcCount++;
                else playerCount++;
            }
        }

        // 如果需要移除回合，则注册到回合管理器的删除列表，延迟在帧末尾删除
        if (npcCount == 0 || playerCount == 0)
        {
            // TurnManager.Instance.AddRemoveTurn(this);
        }
    }

    // ------------------------------------------------------------------------------
    // Help function

    public bool Contain(uint id)
    {
        foreach (var turnItem in turnItemsLists)
        {
            if (turnItem.BIsCharacter && turnItem.character.DynamicId == id) return true;
        }

        return false;
    }

    public bool AddActorByDynamicId(uint id)
    {
        if (Contain(id)) return false;

        Character character = ActorsManagerCenter.Instance.GetActorByDynamicId(id) as Character;
        if (character == null) return false;

        turnItemsLists.Add(new TurnItem(this, character));

        return true;
    }

    public bool RemoveActorByDynamicId(uint id)
    {
        foreach (var turnItem in turnItemsLists)
        {
            if (turnItem.BIsCharacter && turnItem.character.DynamicId == id)
            {
                turnItemsLists.Remove(turnItem);
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------------------
    // Tick Turn functions
    public void PauseTurn()
    {
        preTurnState = turnState;
        turnState = TurnState.TURN_WAIT;
    }

    public void RestartStartTurn()
    {
        turnState = preTurnState;
    }

    public void AddAbilityTask(AbilityBase ability)
    {
        abilityQueue.Enqueue(ability);
    }
    
    public async void EndTurn()
    {
        // 异步的结束回合，可能受到特殊调用导致队列没有执行完毕
        // 等待执行完毕后才转变状态
        bEndTurn = true;

        while (abilityQueue.Count > 0)
        {
            await UniTask.Yield();
        }

        bEndTurn = false;
        SwitchState(TurnState.TURN_END_ACTION);
    }

    public void UpdateView()
    {
    }
}