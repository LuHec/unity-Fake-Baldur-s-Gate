using System;
using System.Collections;
using UnityEngine;

public class Weapon : PickableItem
{
    [SerializeField] private Projectile _projectile;

    public WeaponAttribute WeaponAttributes => _weaponAttribute;
    private WeaponAttribute _weaponAttribute;
    private WaitForSeconds shotWaitTime = new WaitForSeconds(0.1f);
    

    protected override void InitExtend()
    {
        base.InitExtend();
        _pickableItemType = ActorEnumType.PickableItemType.Weapon;
    }

    public void InitWeaponAttribute(WeaponAttribute weaponAttribute)
    {
        _weaponAttribute = weaponAttribute;
    }

    public void SetEquipPosition(Transform equipTrans)
    {
        transform.SetParent(equipTrans);
        transform.localPosition = Vector3.zero;
    }

    public override float GetAttack()
    {
        return _weaponAttribute.damage;
    }

    public override void Attack(GameActor actorAttacked, Action onAttackEnd)
    {
        actorAttacked.Damage(GetAttack());

        StartCoroutine(ShotCoroutine(actorAttacked, onAttackEnd));
    }

    IEnumerator ShotCoroutine(GameActor actorAttacked, Action onAttackEnd)
    {
        int i = 0;
        while (i++ < 2)
        {
            Instantiate(_projectile, transform.position, Quaternion.identity)
                .StartMove(actorAttacked.transform.position);
            
            actorAudio.PlayAttackSFX();

            yield return shotWaitTime;
        }

        // 最后一次生成完成回调
        Instantiate(_projectile, transform.position, Quaternion.identity)
            .StartMove(actorAttacked.transform.position, onAttackEnd);
        actorAudio.PlayAttackSFX();

        yield return null;
    }
}

public class WeaponAttribute
{
    public uint id => _id;
    public string name => _name;
    public float damage => _damage;
    public int aoe => _aoe;
    public float maxDist => _maxDist;

    private uint _id;
    private string _name;
    private float _damage;
    private int _aoe;
    private float _maxDist;


    public WeaponAttribute(uint id, string name, float damage, int aoe, float maxDist)
    {
        this._id = id;
        this._name = name;
        this._damage = damage;
        this._aoe = aoe;
        this._maxDist = maxDist;
    }
}