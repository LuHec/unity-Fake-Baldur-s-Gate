using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(ActorAudio))]
public class GameActor : MonoBehaviour
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
    public MoveComponent moveComponent;

    #endregion

    #region #Init

    private void OnEnable()
    {
        if (actorAudio == null) actorAudio = GetComponent<ActorAudio>();

        if (cmdQue == null) cmdQue = new CommandQueue(this);
        if (moveComponent == null) moveComponent = new MoveComponent(this);
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

    public void SetTurnIntance(TurnInstance turnInstance)
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

    private void Update() 
    {
        moveComponent.UpdateMove();    
    }

    public virtual void ActorUpdate()
    {
    }
    
    
    /// <summary>
    /// 获取攻击力
    /// </summary>
    /// <returns></returns>
    public virtual float GetAttack()
    {
        return 0;
    }

    public virtual void Attack(GameActor target, Action onAttackEnd = null)
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIPanelManager.Instance.ShowPanel<MouseInfoPanel>().UpdateInfoPanel(dynamicID.ToString(), transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIPanelManager.Instance.HidePanel<MouseInfoPanel>();
    }

    public void GetInteractInfo()
    {
    }
}