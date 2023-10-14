public class ScriptObjectDataManager
{
    public CharacterAttributesScriptobjectData CharacterAttrSOData => _characterAttributesScriptobjectData;
    private CharacterAttributesScriptobjectData _characterAttributesScriptobjectData;

    public ScriptObjectDataManager()
    {
        _characterAttributesScriptobjectData = ResourcesLoader.LoadCharacterAttributesData();
        _characterAttributesScriptobjectData.InitDict();
    }
}