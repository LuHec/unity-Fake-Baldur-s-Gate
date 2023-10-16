using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : GameActor
{
    [SerializeField] private Transform weaponTransform;
    private Weapon _weapon;

    #region #TypeInfo

    protected ActorEnumType.CharacterType _characterType;
    public ActorEnumType.CharacterType GetCharacterType() => _characterType;

    #endregion

    protected override void InitExtend()
    {
        _actorEnumType = ActorEnumType.ActorType.Character;
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
    }
}