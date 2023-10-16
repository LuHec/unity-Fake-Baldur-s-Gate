using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : GameActor
{
    [SerializeField] private Transform weaponTransform;
    private Weapon _weapon;

    #region #TypeInfo

    [SerializeField] protected ActorEnumType.CharacterType _characterType;
    [SerializeField] protected ActorEnumType.ActorBattleState _characterBattleState;
    [SerializeField] protected ActorEnumType.ActorHateState _actorHateState;

    public ActorEnumType.CharacterType GetCharacterType() => _characterType;
    public ActorEnumType.ActorBattleState GetCharacterBattleState() => _characterBattleState;
    public ActorEnumType.ActorHateState GetCharacterHateState => _actorHateState;

    #endregion

    #region #Component

    private AIComponent _aiComponent;

    #endregion

    #region #delegate

    public event EventHandler<EventArgsType.ActorAttackingMessage> CharacterAttackingHandler;

    public override void AddListener()
    {
        base.AddListener();
        MessageCenter.Instance.SubmitOnActorAttacking(ref CharacterAttackingHandler);
    }

    #endregion

    protected override void InitExtend()
    {
        _actorEnumType = ActorEnumType.ActorType.Character;

        _aiComponent = new AIComponent(this);
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
        return _weapon.WeaponAttributes.damage;
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

        CharacterAttackingHandler.Invoke(this,
            new EventArgsType.ActorAttackingMessage(Dynamic_Id, actorAttacked.Dynamic_Id));
    }

    public void SetBattleState(ActorEnumType.ActorBattleState state)
    {
        _characterBattleState = state;
    }

    public CommandInstance GenAICommand()
    {
        return _aiComponent.GenAIInstance();
    }
}