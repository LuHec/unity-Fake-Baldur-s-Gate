using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Project.Scripts.LuHeUtility;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 同一时间，一个actor只会存在一个TurnInstance中；如果需要同时存在，合并TurnInstance
/// </summary>
public class TurnInstance
{
    public delegate Task TaskAction();

    public enum TurnState
    {
        TURN_BEGIN,
        TURN_BEGIN_ACTION,
        TURN_BEGIN_ACTION_RUNNING,
        TURN_WAIT_COMMAND,
        TURN_RUNNING,
        TURN_END_ACTION,
        TURN_END_ACTION_RUNNING,
        TURN_END,
    }

    public TurnState CurrentTurnState => turnState;
    private TurnState turnState = TurnState.TURN_BEGIN;
    private CommandCenter commandCenter;

    // private HashSet<uint> conActorDynamicIDSet;
    // private List<uint> conActorDynamicIDs;
    private TListQueue<uint> actorQueue;
    private ActorsManagerCenter actorsManagerCenter;

    // 实时参数
    public uint currentActorId;
    public int turnCount = 0;
    public TListQueue<uint> ActorQueue => actorQueue;
    private Queue<IEnumerator> taskActionQueue = new Queue<IEnumerator>();
    private float turnIntervalCounter = 0;

    #region #Tag

    public bool IsGameModeTurn => isGameModeTurn;
    private bool isGameModeTurn = false;

    #endregion

    #region #delegate

    private EventHandler<EventArgsType.TurnNeedRemoveMessage> turnNeedRemoveHandler;

    // 可以注册角色的buff，进入回合时注册，离开回合时取消
    private Action onEnterTurn;
    private Action onExitTurn;

    private void InitListen()
    {
        commandCenter = CommandCenter.Instance;
        actorsManagerCenter = ActorsManagerCenter.Instance;
        MessageCenter.Instance.ListenOnTurnNeedRemove(ref turnNeedRemoveHandler);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(bool isGameModeTurn = false)
    {
        this.isGameModeTurn = isGameModeTurn;

        InitListen();

        actorQueue = new TListQueue<uint>();
        // conActorDynamicIDs = new List<uint>();
        // conActorDynamicIDSet = new HashSet<uint>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="conActorDynamicIDs"></param>
    /// <param name="isGameModeTurn">是否为手动切换的gamemode，缺省false</param>
    public TurnInstance(List<uint> conActorDynamicIDs, bool isGameModeTurn = false)
    {
        this.isGameModeTurn = isGameModeTurn;

        InitListen();

        actorQueue = new TListQueue<uint>();
        // this.conActorDynamicIDs = new List<uint>();
        // conActorDynamicIDSet = new HashSet<uint>();

        // 添加id，需要从自由列表中移除
        foreach (uint id in conActorDynamicIDs)
        {
            actorsManagerCenter.GetActorByDynamicId(id).InitTurnIntance(this);
            AddActorByDynamicId(id);
        }
    }

    public void SetGameTurnMode(bool pIsGameModeTurn)
    {
        this.isGameModeTurn = pIsGameModeTurn;
    }

    /// <summary>
    /// 依据人物速度对回合进行排序
    /// </summary>
    public void SortByActorSpeed()
    {
        actorQueue.Sort((uint ida, uint idb) =>
        {
            return actorsManagerCenter.GetActorByDynamicId(ida).speed >
                   actorsManagerCenter.GetActorByDynamicId(idb).speed
                ? 1
                : -1;
        });
    }

    public void RebuildActorQueue()
    {
    }


    public bool Contain(uint id)
    {
        return actorQueue.Contain(id);
    }

    /// <summary>
    /// 回合加入新actor，同时为actor设置回合,并清除正在执行的命令
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool AddActorByDynamicId(uint id)
    {
        if (actorQueue.Contain(id)) return false;
        // if (conActorDynamicIDSet.Add(id) == false) return false;
        // conActorDynamicIDs.Add(id);
        actorQueue.Enqueue(id);

        // 初始化actor的回合数据
        var actor = actorsManagerCenter.GetActorByDynamicId(id);
        actor.InitTurnIntance(this);
        actor.ClearCommandCache();

        // 如果在自由模式中需要退出来
        TurnManager.Instance.RemoveFreeModeActorById(id);

        return true;
    }

    public void BackTurn()
    {
    }

    public void RunTurn()
    {
        Character character;
        CommandInstance command;
        switch (turnState)
        {
            case TurnState.TURN_BEGIN:
                turnCount++;
                currentActorId = actorQueue.Dequeue();
                turnState = TurnState.TURN_BEGIN_ACTION;
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;

                // 相关监听注册
                onEnterTurn += character.abilitySystem.onTurnStart;
                onExitTurn += character.abilitySystem.onTurnEnd;

                onEnterTurn?.Invoke();
                break;
            case TurnState.TURN_BEGIN_ACTION:
                turnState = TurnState.TURN_BEGIN_ACTION_RUNNING;
                TurnManager.Instance.StartCoroutines(ExecuteTasks(true));
                break;
            case TurnState.TURN_BEGIN_ACTION_RUNNING:
                break;
            case TurnState.TURN_WAIT_COMMAND:
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;
                // AI和玩家分开处理，玩家需要等待命令
                character.ActorUpdate();
                if (character.GetCommand() != null)
                {
                    character.GetCommand().Excute(character, null);
                    turnState = TurnState.TURN_RUNNING;
                }

                break;
            case TurnState.TURN_RUNNING:
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;

                if (character.GetCommand().isRunning == false)
                {
                    character.ClearCommandCache();
                    turnState = TurnState.TURN_END_ACTION;
                }

                break;
            case TurnState.TURN_END_ACTION:
                turnState = TurnState.TURN_END_ACTION_RUNNING;
                TurnManager.Instance.StartCoroutines(ExecuteTasks(false));
                break;
            case TurnState.TURN_END_ACTION_RUNNING:
                break;
            case TurnState.TURN_END:
                onExitTurn?.Invoke();

                // 取消相关监听
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;
                onEnterTurn -= character.abilitySystem.onTurnStart;
                onExitTurn -= character.abilitySystem.onTurnEnd;

                ResetActorState(currentActorId);
                actorQueue.Enqueue(currentActorId);
                CheckTurn();
                AddTask(TurnInterval());
                turnState = TurnState.TURN_BEGIN;
                break;
        }
    }

    public void AddTask(IEnumerator taskAction)
    {
        taskActionQueue.Enqueue(taskAction);
    }

    /// <summary>
    /// 回合结束后重置角色的资源点
    /// </summary>
    public void ResetActorState(uint id)
    {
    }

    private IEnumerator TurnInterval()
    {
        while (turnIntervalCounter < TurnManager.Instance.WaitTime)
        {
            turnIntervalCounter += Time.deltaTime;
            yield return null;
        }

        turnIntervalCounter = 0;
    }

    private IEnumerator ExecuteTasks(bool begin)
    {
        while (taskActionQueue.Count > 0)
        {
            while (taskActionQueue.Count > 0)
            {
                yield return TurnManager.Instance.StartCoroutine(taskActionQueue.Dequeue());
            }
        }

        turnState = begin ? TurnState.TURN_WAIT_COMMAND : TurnState.TURN_END;
    }

    /// <summary>
    /// 检查回合是否需要被移除
    /// </summary>
    private void CheckTurn()
    {
        // 手动切换的不用检查
        if (IsGameModeTurn) return;

        // 只剩下一个就要移除
        // if (ConActorDynamicIDSet.Count == 1)
        if (actorQueue.Count <= 1)
        {
            turnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
            return;
        }

        // 全是玩家就要移除
        bool needRemove = true;
        // foreach (var id in ConActorDynamicIDSet)
        for (int i = 0; i < actorQueue.Count; i++)
        {
            if (actorsManagerCenter.GetActorByDynamicId(actorQueue[i]).GetActorStateTag() !=
                ActorEnumType.ActorStateTag.Player)
            {
                if (((Character)actorsManagerCenter.GetActorByDynamicId(actorQueue[i])).GetCharacterType() !=
                    ActorEnumType.AIMode.Follow)
                {
                    needRemove = false;
                    break;
                }
            }
        }

        if (needRemove) turnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
    }

    public bool FindActorByDynamicId(uint id)
    {
        // return actorQueue.Exists(x => x == id);
        return actorQueue.Contain(id);
    }

    /// <summary>
    ///  寻找指定id删除actor
    /// </summary>
    /// <param name="id"></param>
    /// <param name="removeFromIDPool">是否执行全局删除actor</param>
    /// <returns></returns>
    public bool RemoveActorByDynamicId(uint id)
    {
        // var pos = conActorDynamicIDs.FindIndex((uint f_id) => { return id == f_id; });
        // if (pos == -1) return false;
        //
        // conActorDynamicIDs.RemoveAt(pos);
        // conActorDynamicIDSet.Remove(id);
        // // 如果删除位置小于指针，需要重定位
        // if (pos < turnActorPtr) BackPtr();
        // return true;

        return actorQueue.Remove(id);
    }
}