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
    public AiComponentGen3 aiComponentGen3;
    public InputComponent inputComponent; 

    #endregion

    #region #delegate

    public Action onEndAction;
    
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

    public void EndAction()
    {
        onEndAction?.Invoke();
    }

    public override void ActorUpdate()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            aiComponentGen3.UpdateAi();
        }
        
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            inputComponent.UpdateInput();
        }
    }

    protected override void InitExtend()
    {
        actorEnumType = ActorEnumType.ActorType.Character;

        abilitySystem = new AbilitySystem(this);
        aiComponentGen3 = new AiComponentGen3(this);
        inputComponent = new InputComponent();

        // 获取能力
        abilitySystem.TryApplyAbility(new Ga_Heal(abilitySystem));
        abilitySystem.TryApplyAbility(new Ga_Attack(abilitySystem));
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
            PickableItem pickableItem = actor as PickableItem;
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

    public override void Attack(GameActor target, Action onAttackEnd)
    {
        if (!ReferenceEquals(weapon, null))
        {
            // 带武器攻击
            weapon.Attack(target, onAttackEnd);
        }
        else
        {
            // 空手攻击
            base.Attack(target, onAttackEnd);
        }
    }
}