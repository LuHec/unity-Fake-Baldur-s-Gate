using UnityEngine;

public class Weapon : PickableItem
{
    public WeaponAttribute WeaponAttributes => _weaponAttribute;
    private WeaponAttribute _weaponAttribute;
    
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

    public virtual void Attack()
    {
        
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