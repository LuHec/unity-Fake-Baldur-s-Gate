using UnityEngine;

public class Weapon : PickableItem
{
    protected override void InitExtend()
    {
        base.InitExtend();

        _pickableItemType = ActorEnumType.PickableItemType.Weapon;
        
    }

    [SerializeField] private WeaponSO _weaponSo;

    public void InitWeapon(WeaponSO weaponSo)
    {
        _weaponSo = weaponSo;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}