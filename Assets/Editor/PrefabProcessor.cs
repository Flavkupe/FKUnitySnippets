using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class PrefabProcessor : MonoBehaviour
{
    [MenuItem("Assets/Export Prefab as Package", true)]
    private static bool ValidateCreateNewPrefab()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/Export Prefab as Package")]
    private static void CreateNewPrefab()
    {
        GameObject selectedPrefab = Selection.activeObject as GameObject;
        if (selectedPrefab == null)
        {
            Debug.LogError("Selected object is not a prefab.");
            return;
        }

        if (!selectedPrefab.name.EndsWith("Demo"))
        {
            Debug.LogError("Selected prefab must end with 'Demo' in the name.");
            return;
        }

        GameObject instance = Instantiate(selectedPrefab);

        RemoveComponents(instance);
        
        string newName = selectedPrefab.name.Replace("Demo", "") + ".prefab";
        string path = AssetDatabase.GetAssetPath(selectedPrefab);
        string newPath = Path.Combine(Path.GetDirectoryName(path), newName);
        PrefabUtility.SaveAsPrefabAsset(instance, newPath);

        ExportPackage(newPath);

        DestroyImmediate(instance);

        AssetDatabase.DeleteAsset(newPath);

        Debug.Log($"New prefab created and exported: ${newPath}");
    }

    private static void RemoveComponents(GameObject gameObject)
    {
        var componentsToRemove = new List<Type>
        {
            typeof(Demo),
            typeof(DemoInputs),
            typeof(DemoObject),
        };

        foreach (var type in componentsToRemove)
        {
            Component[] components = gameObject.GetComponentsInChildren(type, true);
            foreach (var component in components)
            {
                if (component != null)
                {
                    DestroyImmediate(component);
                }
            }
        }
    }

    private static void ExportPackage(string prefabPath)
    {
        var assetPaths = new List<string> { prefabPath };

        string[] dependencies = AssetDatabase.GetDependencies(prefabPath);
        assetPaths.AddRange(dependencies);

        string packagePath = Path.Combine("Assets", "ExportedPackages", Path.GetFileNameWithoutExtension(prefabPath) + ".unitypackage");
        Directory.CreateDirectory(Path.GetDirectoryName(packagePath));
        AssetDatabase.ExportPackage(assetPaths.ToArray(), packagePath, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);

        Debug.Log($"Package exported: ${packagePath}");
    }
}