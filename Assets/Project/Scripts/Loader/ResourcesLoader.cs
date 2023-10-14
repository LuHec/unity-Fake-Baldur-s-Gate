using System.Collections.Generic;
using Excel;
using System.Data;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class ResourcesLoader
{
    public static Object[] LoadAllResources(string path)
    {
        return Resources.LoadAll(path);
    }

    #region #ExcelLoader
    /// <summary>
    /// 加载所有角色的attributes
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static CharacterAttributeSerializable[] LoadAttributesExcel(string filePath)
    {
        List<CharacterAttributeSerializable> attributesList = new List<CharacterAttributeSerializable>();
        int columnNum = 0, rowNum = 0; //excel 行数 列数
        DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);
        //这里i从1开始遍历， 因为第一行是标签名
        for (int i = 1; i < rowNum; i++)
        {
            //如果改行是空行 不保存
            if (IsEmptyRow(collect[i], columnNum)) continue;


            uint.TryParse(collect[i][0].ToString(), out uint id);
            string name = collect[i][1].ToString();
            float.TryParse(collect[i][2].ToString(), out float maxHp);
            int.TryParse(collect[i][3].ToString(), out int maxActPoints);
            uint.TryParse(collect[i][4].ToString(), out uint weaponId);

            CharacterAttributeSerializable attribute = new CharacterAttributeSerializable(id, name, maxHp, maxActPoints, weaponId);
            attributesList.Add(attribute);
        }

        return attributesList.ToArray();
    }

    //判断是否是空行
    static bool IsEmptyRow(DataRow collect, int columnNum)
    {
        for (int i = 0; i < columnNum; i++)
        {
            if (!collect.IsNull(i)) return false;
        }

        return true;
    }

    /// <summary>
    /// 读取excel文件内容获取行数 列数 方便保存
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="columnNum">行数</param>
    /// <param name="rowNum">列数</param>
    /// <returns></returns>
    static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

        DataSet result = excelReader.AsDataSet();
        //Tables[0] 下标0表示excel文件中第一张表的数据
        columnNum = result.Tables[0].Columns.Count;
        rowNum = result.Tables[0].Rows.Count;

        stream.Close();
        return result.Tables[0].Rows;
    }
    #endregion

    #region #ActorLoader
    
    public static Object[] LoadAllControlledActorsResource()
    {
        var actors = Resources.LoadAll("Actors/Character");

        return actors;
    }

    #endregion

    #region #ScriptObjectDataLoader

    public static CharacterAttributesScriptobjectData LoadCharacterAttributesData()
    {
        return AssetDatabase.LoadAssetAtPath<CharacterAttributesScriptobjectData>(@"Assets/Project/ScriptObject/ExcelDataObject/CharacterAttributesDataManager.asset");
    }

    #endregion
}