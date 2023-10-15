using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponAttributesScriptobjectData : ScriptableObject
{
    public WeaponAttributesSerializable[] weaponAttarray;
    public Dictionary<uint, WeaponAttribute> weaponAttDict;

    public void InitArray(WeaponAttributesSerializable[] array)
    {
        weaponAttarray = array;
    }
    
    public void InitDict()
    {
        weaponAttDict = new Dictionary<uint, WeaponAttribute>();   
        foreach (var weapon in weaponAttarray)
        {
            weaponAttDict[weapon.id] = new WeaponAttribute(weapon.id, weapon.name, weapon.damage, weapon.aoe, weapon.maxDist);
        }
    }
}