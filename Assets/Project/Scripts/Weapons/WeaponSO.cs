using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptObject/Weapon")]
public class WeaponSO : ScriptableObject
{
    // 伤害
    public float damage;
    
    // 范围
    public float aoe;
}