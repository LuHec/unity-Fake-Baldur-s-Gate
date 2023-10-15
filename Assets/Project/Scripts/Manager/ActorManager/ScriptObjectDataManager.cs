public class ScriptObjectDataManager
{
    public CharacterAttributesScriptobjectData CharacterAttrSOData => _characterAttributesScriptobjectData;
    private CharacterAttributesScriptobjectData _characterAttributesScriptobjectData;

    public WeaponAttributesScriptobjectData WeaponAttrSOData => _weaponAttributesScriptobjectData;
    private WeaponAttributesScriptobjectData _weaponAttributesScriptobjectData;

    public ScriptObjectDataManager()
    {
        InitCharacterAttributesData();
        InitWeaponAttributesData();
    }

    public void InitCharacterAttributesData()
    {
        _characterAttributesScriptobjectData = ResourcesLoader.LoadCharacterAttributesData();
        _characterAttributesScriptobjectData.InitDict();
    }

    public void InitWeaponAttributesData()
    {
        _weaponAttributesScriptobjectData = ResourcesLoader.LoadWeaponAttributesData();
        _weaponAttributesScriptobjectData.InitDict();
    }
}