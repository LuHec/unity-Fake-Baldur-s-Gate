using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterAttribute
{
    // 读表部分
    private uint _id;
    private string _name;
    private float _maxHp;
    private int _maxActPoints;
    private uint _weaponId;
    
    // 状态部分
    private int _actPoints;
    private float _hp;

    public CharacterAttribute(uint id, string name, float maxHp, int maxActPoints, uint weaponId)
    {
        _id = id;
        _name = name;
        _maxHp = maxHp;
        _maxActPoints = maxActPoints;
        _weaponId = weaponId;

        _hp = _maxHp;
    }

    public uint ID => _id;
    public string Name => _name;
    public float HP => _hp;
    public uint WeaponId => _weaponId;
    
    
    /// <summary>
    /// 返回设置后的血量
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public float SetHP(float val)
    {
        _hp = Mathf.Clamp(val, 0, _maxHp);
        return _hp;
    }

    public float DecreaseHP(float val)
    {
        _hp = Mathf.Clamp(_hp - val, 0, _maxHp);
        return _hp;
    }

    public bool SetWeaponId(uint weaponId)
    {
        _weaponId = weaponId;
        return true;
    }
}
