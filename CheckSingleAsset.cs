using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DependencyChecker
{
    public class CheckSingleAsset : EditorWindow {
        private static float WINDOW_WIDTH = 600;
        private static float WINDOW_HEIGHT = 300;
        public static string resourcePath;
        private static List<string> logText = new List<string>();
        private static List<Object> items = new List<Object>();
        private Vector2 scrollPos = new Vector2(0,0);
        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("obj", GUILayout.Width(30), GUILayout.Height(30)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(resourcePath);
            }
            GUILayout.Label(resourcePath, GUILayout.Width(1000), GUILayout.Height(30));
            GUILayout.EndHorizontal();
            GUILayout.Label("Reference", GUILayout.Width(200), GUILayout.Height(30));
            int nCount = items.Count;
            for(int i = 0;i < nCount;i++)
            {
                GUILayout.BeginHorizontal();
                Object item = items[i];
                string name = logText[i];
                if(GUILayout.Button("go", GUILayout.Width(30), GUILayout.Height(30)))
                {
                    Selection.activeObject = null;
                    Selection.activeObject = item;
                }
                if(Path.GetExtension(name) == ".prefab")
                {
                    if(GUILayout.Button("c", GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        var obj = PrefabUtility.InstantiatePrefab(item) as GameObject;
                        obj = null;
                    }
                }
                GUILayout.Label(name, GUILayout.Width(1000), GUILayout.Height(30));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        public static void Check(string[] searchFiles, Object obj){
            resourcePath = AssetDatabase.GetAssetPath(obj);
            resourcePath = resourcePath.Replace("\\", "/");
            logText.Clear();
            for (int i = 0; i < searchFiles.Length; i++) {
                string subPath = searchFiles[i].Remove(0, Application.dataPath.Length - 6);
                subPath = subPath.Replace("\\", "/");
                if(subPath != resourcePath)
                {
                    string[] deps = AssetDatabase.GetDependencies(subPath, true);
                    foreach (var v in deps)
                    {
                        if(v == resourcePath){
                            logText.Add(subPath);
                        }
                    }
                }
            }
            items.Clear();
            int nCount = logText.Count;
            for(int i = 0;i < nCount; i++)
            {
                items.Add(AssetDatabase.LoadAssetAtPath<Object>(logText[i]));
            }
            OpenWindow();
        }
        private static void OpenWindow()
        {
            var window = (CheckSingleAsset)EditorWindow.GetWindowWithRect(typeof(CheckSingleAsset), new Rect(500, 500, WINDOW_WIDTH, WINDOW_HEIGHT));
            window.titleContent = new GUIContent("CheckSingleAssetDependencies");
            Selection.activeObject = null;
        }
    }
}
