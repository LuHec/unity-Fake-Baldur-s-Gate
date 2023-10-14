using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributesScriptobjectData : ScriptableObject
{
    public Dictionary<uint, CharacterAttributeSerializable> DataDictionary => _dataDictionary;
    private Dictionary<uint, CharacterAttributeSerializable> _dataDictionary;
    public CharacterAttributeSerializable[] _attributes;

    /// <summary>
    /// 创建时使用
    /// </summary>
    /// <param name="characterAttributes"></param>
    public void InitArray(CharacterAttributeSerializable[] characterAttributes)
    {
        _attributes = characterAttributes;
    }

    /// <summary>
    /// 加载后使用
    /// </summary>
    public void InitDict()
    {
        _dataDictionary = new Dictionary<uint, CharacterAttributeSerializable>();

        foreach (var attribute in _attributes)
        {
            _dataDictionary[attribute.id] = attribute;
        }
    }
}