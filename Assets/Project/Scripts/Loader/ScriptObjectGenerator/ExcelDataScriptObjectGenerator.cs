using UnityEditor;
using UnityEngine;

public class ExcelDataScriptObjectGenerator : Editor
{
    public static readonly string CharacterAttributesPath = Application.dataPath + "/ExcelData/CharacterAttribute.xls";
    public static readonly string ScriptObjectDataPath = "Assets/Project/ScriptObject/ExcelDataObject";

    [MenuItem("ExcelData/Load Character Attributes Data")]
    public static void CreateCharacterAttributesData()
    {
        CharacterAttributesScriptobjectData manager = ScriptableObject.CreateInstance<CharacterAttributesScriptobjectData>();
        manager.InitArray(ResourcesLoader.LoadAttributesExcel(CharacterAttributesPath));

        string savePath = ScriptObjectDataPath + "/CharacterAttributesDataManager.asset";

        AssetDatabase.CreateAsset(manager, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("读取数据成功");
    }
}