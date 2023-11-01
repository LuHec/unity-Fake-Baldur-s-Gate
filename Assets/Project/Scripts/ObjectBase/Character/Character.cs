using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Character : GameActor
{
    [SerializeField] private Transform weaponTransform;
    private Weapon _weapon;

    #region #TypeInfo

    [SerializeField] protected ActorEnumType.AIMode aiMode;
    [SerializeField] protected ActorEnumType.ActorBattleState _characterBattleState;
    [SerializeField] protected ActorEnumType.ActorHateState _actorHateState;

    public ActorEnumType.AIMode GetCharacterType() => aiMode;

    /// <summary>
    /// 当前AI状态
    /// </summary>
    /// <param name="type"></param>
    public void SetAIMode(ActorEnumType.AIMode type)
    {
        aiMode = type;
    }

    public ActorEnumType.ActorBattleState GetCharacterBattleState() => _characterBattleState;
    public ActorEnumType.ActorHateState GetCharacterHateState => _actorHateState;

    #endregion

    #region #Command Component

    private bool runningCommand = false;
    private AIComponent _aiComponent;
    private InputCommandsGenerator _inputCommandsGenerator;

    #endregion

    #region #delegate

    public event EventHandler<EventArgsType.ActorAttackingMessage> CharacterAttackingHandler;

    public override void AddListener()
    {
        base.AddListener();
        MessageCenter.Instance.ListenOnActorAttacking(ref CharacterAttackingHandler);
    }

    public override void OnSelected()
    {
        aiMode = ActorEnumType.AIMode.Player;
    }

    #endregion

    protected override void InitExtend()
    {
        _actorEnumType = ActorEnumType.ActorType.Character;

        if (_aiComponent == null)
            _aiComponent = new AIComponent(this);

        if (_inputCommandsGenerator == null)
            _inputCommandsGenerator = new InputCommandsGenerator(this);
    }

    /// <summary>
    /// 装备武器，并设置坐标，如果非武器类型返回false
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    public bool EquipWeapon(GameActor actor)
    {
        if (actor.GetActorType() == ActorEnumType.ActorType.Pickableitem)
        {
            var pickableItem = actor as PickableItem;
            if (pickableItem.GetPickableItemType() == ActorEnumType.PickableItemType.Weapon)
            {
                _weapon = pickableItem as Weapon;

                _weapon.SetEquipPosition(weaponTransform);
                return true;
            }
        }

        return false;
    }

    public override float GetDamage()
    {
        if (_weapon)
            return _weapon.WeaponAttributes.damage;
        else return base.GetDamage();
    }

    public override void Attack(GameActor actorAttacked, Action onAttackEnd)
    {
        if (!ReferenceEquals(_weapon, null))
        {
            _weapon.Attack(actorAttacked, onAttackEnd);
        }
        else
        {
            base.Attack(actorAttacked, onAttackEnd);
        }

        CharacterAttackingHandler?.Invoke(this,
            new EventArgsType.ActorAttackingMessage(Dynamic_Id, actorAttacked.Dynamic_Id));
    }

    public bool IsCommandCacheEmpty()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
            return _aiComponent.CanGenCommandCache;
        else
            return _inputCommandsGenerator.CanGenCommandCache;
    }

    private CommandInstance GenInputCommand()
    {
        CommandInstance cmd = _inputCommandsGenerator.GetCommand();
        if (cmd != null)
        {
        }

        return cmd;
    }

    private CommandInstance GenAICommand()
    {
        return _aiComponent.GetCommand();
    }

    public CommandInstance GenCommandInstance()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            return GenInputCommand();
        }
        else if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            Debug.Log(transform.name + " GenAi");
            return GenAICommand();
        }

        return null;
    }

    public void ClearCommandCache()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            _inputCommandsGenerator.ClearCommandCache();
        }
        else if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            _aiComponent.ClearCommandCache();
        }
    }
}