using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(ActorAudio))]
public class GameActor : MonoBehaviour, IInteractable
{
    #region #Info

    protected ActorEnumType.ActorType actorEnumType;
    [SerializeField] private ActorEnumType.ActorStateTag actorStateTag;

    public ActorEnumType.ActorType GetActorType() => actorEnumType;
    public ActorEnumType.ActorStateTag GetActorStateTag() => actorStateTag;

    // 资源加载id
    public uint id;

    // 运行时id
    private uint dynamicID;

    // 处在的回合实例
    private TurnInstance currentTurn;

    public uint DynamicId => dynamicID;
    public TurnInstance CurrentTurn => currentTurn;

    public float speed;

    [FormerlySerializedAs("_moveSpeed")] [SerializeField]
    private float moveSpeed = 2.0f;

    public CharacterAttribute characterAttribute;

    public Vector3 startPos;
    private CommandQueue cmdQue;
    public CommandQueue CmdQue => cmdQue;

    #endregion

    #region #Component

    public ActorAudio actorAudio;

    #endregion

    #region #Init

    private void OnEnable()
    {
        if (actorAudio == null) actorAudio = GetComponent<ActorAudio>();

        if (cmdQue == null) cmdQue = new CommandQueue(this);
        else cmdQue.Clear();
    }

    /// <summary>
    /// 初始化Actor的属性
    /// </summary>
    /// <param name="newCharacterAttribute"></param>
    public void InitBase(CharacterAttributeSerializable newCharacterAttribute, ActorEnumType.ActorStateTag tag)
    {
        actorStateTag = tag;

        characterAttribute = new CharacterAttribute(newCharacterAttribute.id, newCharacterAttribute.name,
            newCharacterAttribute.maxHp, newCharacterAttribute.maxActPoints, newCharacterAttribute.weaponId);

        startPos.x *= MapSystem.Instance.GetGrid().Cellsize;
        startPos.z *= MapSystem.Instance.GetGrid().Cellsize;
        transform.position = startPos;

        InitExtend();
        AddListener();
    }

    public void InitBase()
    {
        InitExtend();
    }

    /// <summary>
    /// 初始化Actor子类的类型
    /// </summary>
    protected virtual void InitExtend()
    {
    }

    public void InitTurnIntance(TurnInstance turnInstance)
    {
        currentTurn = turnInstance;
    }

    public void InitDynamicId(uint id)
    {
        dynamicID = id;
    }

    #endregion

    #region #delegate

    public event EventHandler<EventArgsType.ActorDieMessage> ActorDiedEventHandler;

    public virtual void AddListener()
    {
        MessageCenter.Instance.ListenOnActorDied(ref ActorDiedEventHandler);
    }

    public virtual void OnSelected()
    {
    }

    #endregion

    #region #AI

    /// <summary>
    /// 设置当前为AI控制还是角色控制
    /// </summary>
    /// <param name="newStateTag"></param>
    public void SetCharacterStateTo(ActorEnumType.ActorStateTag newStateTag)
    {
        actorStateTag = newStateTag;
    }

    #endregion

    public virtual void ActorUpdate()
    {
        
    }
    
    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="cmdInstance"></param>
    /// <returns></returns>
    public bool AddCommand(CommandInstance cmdInstance)
    {
        if (cmdInstance == null || cmdQue.CommandCache == cmdInstance) return false;
        cmdQue.Add(cmdInstance);
        return true;
    }

    /// <summary>
    /// 获取命令队列的队尾
    /// </summary>
    /// <returns></returns>
    public CommandInstance GetCommand() => CmdQue.CommandCache;
    public void ClearCommandCache()
    {
        CmdQue.CommandCache = null;
    }
    
    public Vector3 MoveTo(float x, float z)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), moveSpeed * Time.deltaTime);

        return transform.position;
    }

    /// <summary>
    /// 获取攻击力
    /// </summary>
    /// <returns></returns>
    public virtual float GetAttack()
    {
        return 0;
    }

    public virtual void Attack(GameActor actorAttacked, Action onAttackEnd = null)
    {
        actorAttacked.Damage(GetAttack());
        onAttackEnd?.Invoke();
    }

    public virtual void Damage(float damage)
    {
        characterAttribute.DecreaseHP(damage);
        actorAudio.PlayHitSFX();

        if (characterAttribute.HP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        ActorDiedEventHandler(this, new EventArgsType.ActorDieMessage(dynamicID));
        GetComponentInChildren<MeshRenderer>().materials[0].color = Color.red;
    }

    public virtual void Wait(Action onWaitEnd = null)
    {
        StartCoroutine(WaitCoroutine(onWaitEnd));
    }

    public IEnumerator WaitCoroutine(Action onWaitEnd)
    {
        float t = 1.5f;
        float timmer = 0;
        while (timmer < t)
        {
            timmer += Time.fixedDeltaTime;
            Debug.Log("wait");
            yield return null;
        }

        onWaitEnd?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIPanelManager.Instance.ShowPanel<MouseInfoPanel>().UpdateInfoPanel(transform.name, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIPanelManager.Instance.HidePanel<MouseInfoPanel>();
    }
    
    public void GetInteractInfo()
    {
        
    }
}