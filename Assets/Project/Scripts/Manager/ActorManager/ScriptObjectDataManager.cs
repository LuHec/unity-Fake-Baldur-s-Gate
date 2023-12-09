public class ScriptObjectDataManager
{
    public CharacterAttributesScriptobjectData CharacterAttrSOData => characterAttributesScriptobjectData;
    private CharacterAttributesScriptobjectData characterAttributesScriptobjectData;

    public WeaponAttributesScriptobjectData WeaponAttrSOData => weaponAttributesScriptobjectData;
    private WeaponAttributesScriptobjectData weaponAttributesScriptobjectData;

    public ScriptObjectDataManager()
    {
        InitCharacterAttributesData();
        InitWeaponAttributesData();
    }

    public void InitCharacterAttributesData()
    {
        characterAttributesScriptobjectData = ResourcesLoader.LoadCharacterAttributesData();
        characterAttributesScriptobjectData.InitDict();
    }

    public void InitWeaponAttributesData()
    {
        weaponAttributesScriptobjectData = ResourcesLoader.LoadWeaponAttributesData();
        weaponAttributesScriptobjectData.InitDict();
    }
}