using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurnInstance2
{
    // ------------------------------------------------------------------------------
    // Actors Parameters
    private List<Character> characterLists = new List<Character>();
    private Queue<Character> characterQueue = new Queue<Character>();

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
        TURN_BEGIN = 0,
        TURN_BEGIN_ACTION,
        TURN_BEGIN_ACTION_RUNNING,
        TURN_RUNNING,
        TURN_END_ACTION,
        TURN_END_ACTION_RUNNING,
        TURN_END,
    }
    
    private Queue<Action> actionQueue = new Queue<Action>();
    private TurnDelegate turnDelegate = new TurnDelegate();

    private int turnCounter = 0;
    private TurnState turnState = TurnState.TURN_BEGIN;
    private bool bActionsFinished = true;
    

    // ------------------------------------------------------------------------------
    // Initialize Functions
    public TurnInstance2()
    {
        Initialize();
    }

    private void Initialize()
    {
        turnDelegate.onStartAction += ExecuteActions;
        turnDelegate.onActorEndAct += OnActorEndAct;
    }

    // ------------------------------------------------------------------------------
    // Run Turn Functions
    public void UpdateTurn()
    {
        if (characterQueue.Count <= 0) return;
        Debug.Log(turnState);

        switch (turnState)
        {
            case TurnState.TURN_BEGIN:
                // 角色监听事件
                turnDelegate.onTurnStart += characterQueue.Peek().abilitySystem.abilityDelegate.onTurnStart;
                turnDelegate.onTurnEnd += characterQueue.Peek().abilitySystem.abilityDelegate.onTurnEnd;
                // 监听角色结束行动
                characterQueue.Peek().abilitySystem.abilityDelegate.onActEnd += turnDelegate.onActorEndAct;
                
                turnDelegate.onTurnStart?.Invoke();
                
                turnState = TurnState.TURN_BEGIN_ACTION;
                break;

            case TurnState.TURN_BEGIN_ACTION:
                bActionsFinished = false;
                ExecuteActions();
                
                turnState = TurnState.TURN_BEGIN_ACTION_RUNNING;
                break;

            case TurnState.TURN_BEGIN_ACTION_RUNNING:
                if (bActionsFinished) 
                    turnState = TurnState.TURN_RUNNING;
                break;

            case TurnState.TURN_RUNNING:
                // Execute Actor Command
                characterQueue.Peek().ActorUpdate();
                break;

            case TurnState.TURN_END_ACTION:
                bActionsFinished = false;
                ExecuteActions();
                
                turnState = TurnState.TURN_END_ACTION_RUNNING;
                break;

            case TurnState.TURN_END_ACTION_RUNNING:
                if (bActionsFinished)
                    turnState = TurnState.TURN_END;
                break;

            case TurnState.TURN_END:
                turnCounter++;
                // 清除委托
                turnDelegate.onTurnStart -= characterQueue.Peek().abilitySystem.abilityDelegate.onTurnStart;
                turnDelegate.onTurnEnd -= characterQueue.Peek().abilitySystem.abilityDelegate.onTurnEnd;
                characterQueue.Peek().abilitySystem.abilityDelegate.onActEnd -= turnDelegate.onActorEndAct;

                // 回合结束后进行重排序
                SortByActorSpeed();
                
                // 等待一秒
                actionQueue.Enqueue(WaitOneSecond);
                
                turnState = TurnState.TURN_BEGIN; 
                break;
        }
    }
    
    private async void ExecuteActions()
    {
        while (actionQueue.Count > 0)
        {
            await UniTask.RunOnThreadPool(actionQueue.Peek());

            // 延迟一秒,退出已完成的任务
            await UniTask.WaitForSeconds(1);
            actionQueue.Dequeue();
        }

        bActionsFinished = true;
    }
    
    // 角色主动结束行动
    private void OnActorEndAct()
    {
        turnState = TurnState.TURN_END_ACTION;
    }

    private async void WaitOneSecond()
    {
        await UniTask.WaitForSeconds(1);
    }
    
    // ------------------------------------------------------------------------------
    // Update info

    public void SortByActorSpeed()
    {
        // 对list重新排序，但是要去除当前回合的角色，并放置在队尾
        Character currentCharacter = characterQueue.Peek();
        
        characterLists.Sort();
        characterQueue.Clear();

        foreach (var character in characterLists)
        {
            if (character != currentCharacter)
            {
                characterQueue.Enqueue(character);
            }
        }
        
        characterQueue.Enqueue(currentCharacter);
    }
    
    
}