using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Character : GameActor
{
    [SerializeField] private Transform weaponTransform;
    private Weapon weapon;

    #region #TypeInfo

    [SerializeField] protected ActorEnumType.AIMode aiMode;
    [SerializeField] protected ActorEnumType.ActorBattleState characterBattleState;
    [SerializeField] protected ActorEnumType.ActorHateState actorHateState;

    public ActorEnumType.AIMode GetCharacterType() => aiMode;

    /// <summary>
    /// 当前AI状态
    /// </summary>
    /// <param name="type"></param>
    public void SetAIMode(ActorEnumType.AIMode type)
    {
        aiMode = type;
    }

    public ActorEnumType.ActorBattleState GetCharacterBattleState() => characterBattleState;
    public ActorEnumType.ActorHateState GetCharacterHateState => actorHateState;

    #endregion

    #region #Component
    
    public AbilitySystem abilitySystem;
    private AIComponent aiComponent;

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

    public override void ActorUpdate()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            if (CmdQue.CommandCache == null)
            {
                AddCommand(aiComponent.GetCommand());
            }
        }
    }

    protected override void InitExtend()
    {
        actorEnumType = ActorEnumType.ActorType.Character;

        abilitySystem ??= new AbilitySystem(this);
        
        aiComponent ??= new AIComponent(this);

        // 获取治疗能力
        abilitySystem.TryApplyAbility(new Ga_Heal(this));
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
                weapon = pickableItem as Weapon;

                weapon.SetEquipPosition(weaponTransform);
                return true;
            }
        }

        return false;
    }

    public override float GetAttack()
    {
        if (weapon)
            return weapon.WeaponAttributes.damage;
        else return base.GetAttack();
    }

    public override void Attack(GameActor actorAttacked, Action onAttackEnd)
    {
        if (!ReferenceEquals(weapon, null))
        {
            weapon.Attack(actorAttacked, onAttackEnd);
        }
        else
        {
            base.Attack(actorAttacked, onAttackEnd);
        }

        CharacterAttackingHandler?.Invoke(this,
            new EventArgsType.ActorAttackingMessage(DynamicId, actorAttacked.DynamicId));
    }

    public bool IsCommandCacheEmpty()
    {
        if (CmdQue.Size() == 0) return true;
        return CmdQue.Back().isRunning == false;
        // if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        //     return aiComponent.CanGenCommandCache;
        // else
        //     return inputCommandsGenerator.CanGenCommandCache;
    }

    // private CommandInstance GenInputCommand()
    // {
    //     CommandInstance cmd = inputCommandsGenerator.GetCommand();
    //     AddCommand(cmd);
    //
    //     return cmd;
    // }
}