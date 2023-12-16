using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierPool
{
    private static ModifierPool instance = new ModifierPool();
    public static ModifierPool Instance => instance;

    public Dictionary<string, ModifierCfgBase> cfgDictionary = new Dictionary<string, ModifierCfgBase>();

    private ModifierPool()
    {
        var modifiers = Resources.LoadAll<ModifierCfgBase>("AbilitySystem/ModiferCfg");
        foreach (var modifer in modifiers)
        {
            cfgDictionary.Add(modifer.name, modifer);
        }
    }

    public Modifier CreateModifier(string name, GameActor owner, GameActor target)
    {
        if (cfgDictionary.ContainsKey(name))
            return new Modifier(cfgDictionary[name], owner, target);
        return null;
    }
}