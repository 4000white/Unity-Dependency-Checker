using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace DependencyChecker
{
    public class CheckDependency : EditorWindow
    {
        private const int DEFAULT_ASSETS_NUM = 2;
        private const int DEFAULT_DEPENDENCY_NUM = 2;
        private const float HEIGHT = 25;
        private const float BUTTON_WIDTH = 25;
        private const float PAD = 0.5f;
        private const float LABEL_WIDTH_1 = 100;
        private const float LABEL_WIDTH_2 = 300;
        private const string LABEL_CACHE_1 = "AssetFold";
        private const string LABEL_CACHE_2 = "CheckFold";
        private const string LABEL_CACHE_3 = "Check";
        private const string LABEL_CACHE_4 = "+";

        public static string[] assetFiles;
        public static string[] dependencyFiles;

        private List<Object> assets = new List<Object>();
        private List<Object> dependencies = new List<Object>();
        private Rect r = new Rect(0, 0, HEIGHT, 0);

        [MenuItem("Window/CheckDependency")]
        private static void OpenWindow()
        {
            CheckDependency window = (CheckDependency)EditorWindow.GetWindowWithRect(typeof(CheckDependency), new Rect(Screen.width/2, Screen.height/2, 400, 400));
            window.minSize = new Vector2(300, 300);
            window.titleContent = new GUIContent("CheckDependency");
            window.Init();
        }

        private void Init()
        {
            assets.Clear();
            for (int i = 0; i < DEFAULT_ASSETS_NUM; i++)
            {
                assets.Add(null);
            }
            dependencies.Clear();
            for (int i = 0; i < DEFAULT_DEPENDENCY_NUM; i++)
            {
                dependencies.Add(null);
            }
        }

        private void OnGUI()
        {
            r.x = 0;
            r.y = 0;
            r.height = HEIGHT;
            r.width = 0;

            r.width = LABEL_WIDTH_1;
            GUI.Label(r, LABEL_CACHE_1);

            r.width = LABEL_WIDTH_2;
            for(int i = 0;i< assets.Count; i++)
            {
                r.y += HEIGHT + PAD;
                assets[i] = EditorGUI.ObjectField(r, assets[i], typeof(Object), false);
            }

            r.x += r.width + PAD;
            r.width = BUTTON_WIDTH;
            if(GUI.Button(r, LABEL_CACHE_4))
            {
                assets.Add(null);
            }
            r.y += HEIGHT + PAD;
            r.x = 0;

            r.width = LABEL_WIDTH_1;
            GUI.Label(r, LABEL_CACHE_2);

            r.width = LABEL_WIDTH_2;
            for (int i = 0; i < dependencies.Count; i++)
            {
                r.y += HEIGHT + PAD;
                dependencies[i] = EditorGUI.ObjectField(r, dependencies[i], typeof(Object), false);
            }

            r.x += r.width + PAD;
            r.width = BUTTON_WIDTH;
            if (GUI.Button(r, LABEL_CACHE_4))
            {
                dependencies.Add(null);
            }
            r.x = 0;
            r.y += HEIGHT + PAD;
            r.width = LABEL_WIDTH_1;
            if (GUI.Button(r, LABEL_CACHE_3))
            {
                assetFiles = GetAllFiles(assets.ToArray());
                dependencyFiles = GetAllFiles(dependencies.ToArray());
                CheckAssets.CheckAsset(assetFiles, dependencyFiles);
            }
        }

        private static string[] GetAllFiles(Object[] assets)
        {
            List<string> allFiles = new List<string>();
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] != null)
                {
                    var path = AssetDatabase.GetAssetPath(assets[i]);
                    if(assets[i].GetType() == typeof(DefaultAsset)){
                        string[] files = IOUtil.GetFiles(path);
                        for(int j = 0;j<files.Length;j++){
                            allFiles.Add(files[j]);
                        }
                    }
                    else
                    {
                        allFiles.Add(Path.GetFullPath(path));
                    }
                }
            }
            return allFiles.ToArray();
        }
    }
}
