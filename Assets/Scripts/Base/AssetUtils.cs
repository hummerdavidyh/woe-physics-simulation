using System;
using System.IO;

using UnityEngine;


public class AssetUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resourcePath"></param>
    /// <param name="fileName"></param>
    /// <param name="saveAssetDatabase"></param>
    /// <param name="refreshAssetDatabase"></param>
    /// <returns></returns>
    public static T GetScriptableObject<T>(
        string resourcePath, 
        string fileName,
        bool saveAssetDatabase = true,
        bool refreshAssetDatabase = true) where T : ScriptableObject {

        if (string.IsNullOrEmpty(resourcePath)
         || string.IsNullOrEmpty(fileName)) {
            return null;
        }

        T obj =  Resources.Load<T>(fileName);
        if (obj == null) {

            string simpleResourcesPath = resourcePath.Replace(
                resourcePath.Substring(0, resourcePath.LastIndexOf("Resources", StringComparison.Ordinal)), "");
            simpleResourcesPath = simpleResourcesPath.Replace("Resources", "").Remove(0, 1);
            string combinedPath = Path.Combine(simpleResourcesPath, fileName);
            combinedPath = combinedPath.Replace(@"\\", @"\");
            combinedPath = combinedPath.Replace(@"\", "/");

            obj = Resources.Load<T>(combinedPath);
        }
        if (obj != null)
            return obj;

        return obj;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ClearPath(string path) {
        if (!path[path.Length - 1].Equals(@"\")) {
            path += @"\";
        }

        path.Replace(@"\\", @"\");
        path.Replace(@"\", @"/");

        return path;
    }

    public static string NormalizePath(string path) {
        string normalPath = path.Replace(@"\\", @"\");
        normalPath = normalPath.Replace(@"\", @"/");

        return normalPath;
    }

    public static string RemoveFileExtension(string fileName) {
        int pos = fileName.LastIndexOf('.');
        return fileName.Substring(0, pos);
    }

    public static string GetFileName(string fullPath) {
        string normalPath = NormalizePath(fullPath);
        return normalPath.Substring(normalPath.LastIndexOf("/") + 1);
    }
    

#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="relativePath"></param>
    /// <param name="fileName"></param>
    /// <param name="extension"></param>
    /// <param name="saveAssetDatabase"></param>
    /// <param name="refreshAssetDatabase"></param>
    /// <returns></returns>
    public static T CreateAsset<T>(
        string relativePath, 
        string fileName, 
        string extension = ".asset",
        bool saveAssetDatabase = true,
        bool refreshAssetDatabase = true) where T : ScriptableObject {

        if (string.IsNullOrEmpty(relativePath)
         || string.IsNullOrEmpty(fileName))
            return null;
        
        // 产生一个ScriptableObject实例
        T obj = ScriptableObject.CreateInstance<T>();

        UnityEditor.AssetDatabase.AddObjectToAsset(obj, relativePath + fileName + extension);
        UnityEditor.EditorUtility.SetDirty(obj);
        if (saveAssetDatabase)
            UnityEditor.AssetDatabase.SaveAssets();
        if (refreshAssetDatabase)
            UnityEditor.AssetDatabase.Refresh();

        return obj;
    }
#endif
}
