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

    #region #Command Component

    private bool runningCommand = false;
    private AIComponent aiComponent;
    private InputCommandsGenerator inputCommandsGenerator;

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
        actorEnumType = ActorEnumType.ActorType.Character;

        if (aiComponent == null)
            aiComponent = new AIComponent(this);

        if (inputCommandsGenerator == null)
            inputCommandsGenerator = new InputCommandsGenerator(this);
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

    public override float GetDamage()
    {
        if (weapon)
            return weapon.WeaponAttributes.damage;
        else return base.GetDamage();
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
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
            return aiComponent.CanGenCommandCache;
        else
            return inputCommandsGenerator.CanGenCommandCache;
    }

    private CommandInstance GenInputCommand()
    {
        CommandInstance cmd = inputCommandsGenerator.GetCommand();
        AddCommand(cmd);

        return cmd;
    }

    private CommandInstance GenAICommand()
    {
        CommandInstance cmd = aiComponent.GetCommand();
        AddCommand(cmd);

        return cmd;
    }

    public CommandInstance GetCommandInstance()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            return GenInputCommand();
        }

        if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            return GenAICommand();
        }

        return null;
    }

    public void ClearCommandCache()
    {
        if (GetActorStateTag() == ActorEnumType.ActorStateTag.Player)
        {
            inputCommandsGenerator.ClearCommandCache();
        }
        else if (GetActorStateTag() == ActorEnumType.ActorStateTag.AI)
        {
            aiComponent.ClearCommandCache();
        }
    }
}