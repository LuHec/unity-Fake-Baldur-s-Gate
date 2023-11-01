using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(ActorAudio))]
public class GameActor : MonoBehaviour
{
    #region #Info

    protected ActorEnumType.ActorType _actorEnumType;
    [SerializeField] private ActorEnumType.ActorStateTag _actorStateTag;

    public ActorEnumType.ActorType GetActorType() => _actorEnumType;
    public ActorEnumType.ActorStateTag GetActorStateTag() => _actorStateTag;


    // 资源加载id
    public uint id;

    // 运行时id
    private uint dynamic_id;

    // 处在的回合实例
    private TurnInstance _currentTurn;

    public uint Dynamic_Id => dynamic_id;
    public TurnInstance CurrentTurn => _currentTurn;

    public float speed;

    // [SerializeField] private PlacedObjectTypeSO _placedObjectTypeSo;
    [SerializeField] private float _moveSpeed = 2.0f;

    public CharacterAttribute characterAttribute;
    // public PlacedObjectTypeSO PlacedObject => _placedObjectTypeSo;

    public Vector3 startPos;
    private CommandQueue _cmdQue;
    public CommandQueue CmdQue => _cmdQue;

    private MapSystem _mapSystem;

    #endregion

    #region #Component

    public ActorAudio actorAudio => _actorAudio;
    private ActorAudio _actorAudio;

    #endregion

    #region #Init

    private void OnEnable()
    {
        if (actorAudio == null) _actorAudio = GetComponent<ActorAudio>();

        if (_cmdQue == null) _cmdQue = new CommandQueue(this);
        else _cmdQue.Clear();
    }

    /// <summary>
    /// 初始化Actor的属性
    /// </summary>
    /// <param name="newCharacterAttribute"></param>
    public void InitBase(CharacterAttributeSerializable newCharacterAttribute, ActorEnumType.ActorStateTag tag)
    {
        _actorStateTag = tag;

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
        _currentTurn = turnInstance;
    }

    public void InitDynamicId(uint id)
    {
        dynamic_id = id;
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
        _actorStateTag = newStateTag;
    }

    #endregion

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="cmdInstance"></param>
    /// <returns></returns>
    public bool AddCommand(CommandInstance cmdInstance)
    {
        if (cmdInstance == null) return false;
        if (_cmdQue.Back() == cmdInstance) return false;
        _cmdQue.Add(cmdInstance);
        return true;
    }

    /// <summary>
    /// 获取命令队列的队尾
    /// </summary>
    /// <returns></returns>
    public CommandInstance GetCommand() => _cmdQue.Back();

    /// <summary>
    /// 输入XZ，返回世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 CalculateMoveTo(float x, float z)
    {
        return Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed * Time.deltaTime);
    }

    public void DirectMoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 MoveTo(float x, float z)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, new Vector3(x, transform.position.y, z), _moveSpeed);

        return transform.position;
    }

    /// <summary>
    /// 获取攻击力
    /// </summary>
    /// <returns></returns>
    public virtual float GetDamage()
    {
        return 0;
    }

    public virtual void Attack(GameActor actorAttacked, Action onAttackEnd = null)
    {
        actorAttacked.Hurt(GetDamage());
        onAttackEnd?.Invoke();
    }

    public virtual void Hurt(float damage)
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
        ActorDiedEventHandler(this, new EventArgsType.ActorDieMessage(dynamic_id));
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
}