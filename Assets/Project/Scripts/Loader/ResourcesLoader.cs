using UnityEngine;

public static class ResourcesLoader
{
    public static Object[] LoadAllResources(string path)
    {
        return Resources.LoadAll(path);
    }
}