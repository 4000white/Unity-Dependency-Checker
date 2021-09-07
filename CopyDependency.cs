using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;

namespace DependencyChecker
{
    public class CopyDependency : EditorWindow
    {
        private static List<string> includeList = new List<string>{ ".prefab" , ".mat", ".anim", ".controller"};
        private static List<string> ignoreList = new List<string>{".meta", ".DS_Store"};
        private const string strCache1 = "Target Files";
        private const string strCache2 = "Change Dependencies From";
        private const string strCache3 = "To";
        Object depFold;
        Object copyFold;
        Object copyDepFold;
        private static string depPath = "";
        private static string copyPath = "";
        private static string copyDepPath = "";       

        [MenuItem("Window/CopyDependency")]
		private static void Init()
		{
			CopyDependency window = (CopyDependency)EditorWindow.GetWindow(typeof(CopyDependency));
			window.minSize = new Vector2(300, 300);
            window.titleContent = new GUIContent("CopyDependency");
		}

        private void OnGUI()
        {
            EditorGUILayout.LabelField(strCache1);
            copyFold = EditorGUILayout.ObjectField(copyFold, typeof(Object), false);
            if (copyFold != null)
            {
                if (copyFold.GetType() == typeof(DefaultAsset))
                {
                    copyPath = AssetDatabase.GetAssetPath((DefaultAsset)copyFold);
                }
            }

            EditorGUILayout.LabelField(strCache2);
            depFold = EditorGUILayout.ObjectField(depFold, typeof(Object), false);
            if (depFold != null)
            {
                if (depFold.GetType() == typeof(DefaultAsset))
                {
                    depPath = AssetDatabase.GetAssetPath((DefaultAsset)depFold);
                }
            }

            EditorGUILayout.LabelField(strCache3);
            copyDepFold = EditorGUILayout.ObjectField(copyDepFold, typeof(Object), false);
            if (copyDepFold != null)
            {
                if (copyDepFold.GetType() == typeof(DefaultAsset))
                {
                    copyDepPath = AssetDatabase.GetAssetPath((DefaultAsset)copyDepFold);
                }
            }

            if (GUILayout.Button("Copy", GUILayout.Width(50), GUILayout.Height(50)))
            {
                GetDependencyMap();
                ReplaceDependencyGuid();
            }
        }

        /// <summary>
        /// key:old guid value:new guid
        /// </summary>
        private Dictionary<string, string> guidMap = new Dictionary<string, string>();
        private void GetDependencyMap()
        {
            guidMap.Clear();
            var fullPath = Path.GetFullPath(depPath);
            var filePaths = IOUtil.GetFiles(fullPath);
            var length = fullPath.Length + 1;
            foreach (var filePath in filePaths)
            {
                string extension = Path.GetExtension(filePath);
                if (!ignoreList.Contains(extension))
                {
                    string assetPath = IOUtil.GetRelativeAssetPath(filePath);
                    string relativePath = filePath.Remove(0, length);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    string copyPath = copyDepPath + "/" + relativePath;
                    string copyGuid = AssetDatabase.AssetPathToGUID(copyPath);
                    if(copyGuid != null)
                    {
                        guidMap[guid] = copyGuid;
                    }
                }
            }
        }

        private void ReplaceDependencyGuid()
        {
            var fullPath = Path.GetFullPath(copyPath);
            var filePaths = IOUtil.GetFiles(fullPath);
            foreach (var filePath in filePaths)
            {
                string extension = Path.GetExtension(filePath);
                if (includeList.Contains(extension))
                {
                    var assetPath = IOUtil.GetRelativeAssetPath(filePath);
                    string[] deps = AssetDatabase.GetDependencies(assetPath, true);
                    var fileString = File.ReadAllText(filePath);
                    bool bChanged = false;
                    foreach (var v in deps)
                    {
                        var guid = AssetDatabase.AssetPathToGUID(v);
                        if (guidMap.ContainsKey(guid))
                        {
                            if (Regex.IsMatch(fileString, guid))
                            {
                                fileString = Regex.Replace(fileString, guid, guidMap[guid]);
                                bChanged = true;
                            }
                        }
                    }
                    if(bChanged){
                        File.WriteAllText(filePath, fileString);
                    }
                }
            }
        }
    }
}
