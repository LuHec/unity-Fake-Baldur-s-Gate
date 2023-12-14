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

    enum TurnState
    {
        TURN_BEGIN,
        TURN_BEGIN_ACTION,
        TURN_RUNNING,
        TURN_END_ACTION,
        TURN_END,
    }

    private TurnState turnState = TurnState.TURN_BEGIN;
    private CommandCenter commandCenter;

    // private HashSet<uint> conActorDynamicIDSet;
    // private List<uint> conActorDynamicIDs;
    private TListQueue<uint> actorQueue;
    private ActorsManagerCenter actorsManagerCenter;
    private int turnActorPtr = 0;

    // 实时参数
    public uint currentActorId;
    public int turnCount = 0;
    public TListQueue<uint> ActorQueue => actorQueue;
    private Queue<IEnumerator> taskActionQueue = new Queue<IEnumerator>();
    private bool hasStartedTask;
    private float turnIntervalCounter = 0;

    #region #Tag

    public bool IsGameModeTurn => isGameModeTurn;
    private bool isGameModeTurn = false;

    #endregion

    #region #delegate

    private EventHandler<EventArgsType.TurnNeedRemoveMessage> TurnNeedRemoveHandler;
    
    // 可以注册角色的buff，进入回合时注册，离开回合时取消
    private Action onEnterTurn;
    private Action onExitTurn;

    private void InitListen()
    {
        commandCenter = CommandCenter.Instance;
        actorsManagerCenter = ActorsManagerCenter.Instance;
        MessageCenter.Instance.ListenOnTurnNeedRemove(ref TurnNeedRemoveHandler);
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

    public void SetGameTurnMode(bool isGameModeTurn)
    {
        this.isGameModeTurn = isGameModeTurn;
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

    public void NextTurn()
    {
        // PushPtr();
        actorQueue.Enqueue(currentActorId);
        // CheckTurn();
    }

    public void BackTurn()
    {
        // BackPtr();
        //
        // foreach (var uid in conActorDynamicIDs)
        // {
        //     Character character = ActorsManagerCenter.Instance.GetActorByDynamicId(uid) as Character;
        //     character.CmdQue.Undo();
        // }
    }

    public void RunTurn()
    {
        Character character;
        switch (turnState)
        {
            case TurnState.TURN_BEGIN:
                currentActorId = actorQueue.Dequeue();
                turnState = TurnState.TURN_BEGIN_ACTION;
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;
                onEnterTurn += character.buffSystem.OnTurnEnter;
                onExitTurn += character.buffSystem.OnTurnExit;

                onEnterTurn();
                break;
            case TurnState.TURN_BEGIN_ACTION:
                if (!hasStartedTask)
                {
                    hasStartedTask = true;
                    TurnManager.Instance.StartCoroutines(ExecuteTasks());
                }

                break;
            case TurnState.TURN_RUNNING:
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;
                character.ActorUpdate();

                // AI和玩家分开处理，玩家需要等待命令
                var command = character.GetCommand();
                if (command == null) return;

                if (command.IsRunning)
                {
                    commandCenter.Excute(command, character, null);
                }
                else
                {
                    character.ClearCommandCache();
                    turnState = TurnState.TURN_END_ACTION;
                }

                break;
            case TurnState.TURN_END_ACTION:
                turnState = TurnState.TURN_END;
                break;
            case TurnState.TURN_END:
                onExitTurn();
                character = actorsManagerCenter.GetActorByDynamicId(currentActorId) as Character;
                onEnterTurn -= character.buffSystem.OnTurnEnter;
                onExitTurn -= character.buffSystem.OnTurnExit;
                
                actorQueue.Enqueue(currentActorId);
                CheckTurn();
                AddTask(TurnInterval());
                hasStartedTask = false;
                turnState = TurnState.TURN_BEGIN;
                
                
                break;
        }
    }

    public void AddTask(IEnumerator taskAction)
    {
        taskActionQueue.Enqueue(taskAction);
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

    private IEnumerator ExecuteTasks()
    {
        while (taskActionQueue.Count > 0)
        {
            while (taskActionQueue.Count > 0)
            {
                yield return TurnManager.Instance.StartCoroutine(taskActionQueue.Dequeue());
            }
        }

        turnState = TurnState.TURN_RUNNING;
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
            TurnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
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

        if (needRemove) TurnNeedRemoveHandler(this, new EventArgsType.TurnNeedRemoveMessage(this));
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