using UnityEditor;
using UnityEngine;

public class ExcelDataScriptObjectGenerator : Editor
{
    public static readonly string ScriptObjectDataPath = "Assets/Project/ScriptObject/ExcelDataObject";


    public static readonly string CharacterAttributesPath = Application.dataPath + "/ExcelData/CharacterAttribute.xls";

    [MenuItem("ExcelData/Load Character Attributes Data")]
    public static void CreateCharacterAttributesData()
    {
        CharacterAttributesScriptobjectData manager =
            ScriptableObject.CreateInstance<CharacterAttributesScriptobjectData>();
        manager.InitArray(ResourcesLoader.LoadCharacterAttributesExcel(CharacterAttributesPath));

        string savePath = ScriptObjectDataPath + "/CharacterAttributesDataManager.asset";

        AssetDatabase.CreateAsset(manager, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("读取数据成功");
    }


    public static readonly string WeaponAttributesPath = Application.dataPath + "/ExcelData/WeaponAttribute.xls";

    [MenuItem("ExcelData/Load Weapon AttributesData")]
    public static void CreateWeaponAttributesData()
    {
        WeaponAttributesScriptobjectData manager =
            ScriptableObject.CreateInstance<WeaponAttributesScriptobjectData>();
        manager.InitArray(ResourcesLoader.LoadWeaponAttributesExcel(WeaponAttributesPath));

        string savePath = ScriptObjectDataPath + "/WeaponAttributeDataManager.asset";

        AssetDatabase.CreateAsset(manager, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("读取数据成功");
    }
}