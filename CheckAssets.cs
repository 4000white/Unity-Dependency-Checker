using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace DependencyChecker
{
	public class CheckAssets : EditorWindow {
		private static List<string> ignoreList = new List<string>{".meta", ".DS_Store"};
		internal const int ORDER = 50;
		private const float WINDOW_WIDTH = 800;
		private const float WINDOW_HEIGHTTH = 800;
		private static float scrollHeight = 0;
		private const float scrollWidth = 1000;
		private const float PAD = 0.8f;
		private const float HEIGHT = 20;
		private const float BUTTON_WIDTH = 20;
		private const float LABEL_WIDTH = 800;
		private const string LABEL_CACHE_1 = "";
		private const string LABEL_CACHE_2 = "Not Dependency";
		private const string LABLE_CACHE_3 = "Dependency";

		private static GUIStyle textStyle;
		private static Vector2 scrollPosition = Vector2.zero;
		private static int rowCount1 = 0;
		private static int rowCount2 = 0;
		private static Rect r = new Rect(0, 0, BUTTON_WIDTH, HEIGHT);

		private static string searchPath;
		private static string resourcePath;
		private static List<string> logText = new List<string>();
		private static List<string> logText2 = new List<string>();

		[MenuItem("Assets/SetSearchPath", false, ORDER)]
		private static void SetSearchPath()
		{
			if (Selection.activeObject != null)
			{
				if (Selection.activeObject.GetType() == typeof(DefaultAsset))
				{
					searchPath = AssetDatabase.GetAssetPath((DefaultAsset)Selection.activeObject);
					CheckDependency.assetFiles = IOUtil.GetFiles(searchPath);
				}
			}
		}
		[MenuItem("Assets/CheckAssets", false, ORDER + 1)]
		private static void OnClick(){
			if (searchPath == "")
			{
				return;
			}
			if (Selection.activeObject != null ) {
				if(Selection.activeObject.GetType () == typeof(DefaultAsset)){
					resourcePath = AssetDatabase.GetAssetPath((DefaultAsset)Selection.activeObject);
					CheckDependency.dependencyFiles = IOUtil.GetFiles(resourcePath);
					CheckAsset(CheckDependency.assetFiles, CheckDependency.dependencyFiles);
					Selection.activeObject = null;
				}
				else
				{
					CheckSingleAsset.Check(CheckDependency.assetFiles, Selection.activeObject);
				}
			}
		}
		private static void OpenWindow()
		{
			var window = (CheckAssets)EditorWindow.GetWindowWithRect(typeof(CheckAssets), new Rect(300, 300, WINDOW_WIDTH, WINDOW_HEIGHTTH));
			window.titleContent = new GUIContent("CheckAssetDependency");
			Selection.activeObject = null;
		}

		private void OnGUI()
		{
			textStyle = GUI.skin.GetStyle("Label");
			textStyle.alignment = TextAnchor.MiddleLeft;
			scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, WINDOW_WIDTH, WINDOW_HEIGHTTH), scrollPosition, new Rect(0f, 0f, scrollWidth, scrollHeight), true, true);
			r.x = 0;
			r.y = 0;
			r.width = LABEL_WIDTH;
            if (CheckInsideScroll())
            {
				GUI.Label(r, LABEL_CACHE_2, textStyle);
			}
			r.y += HEIGHT + PAD;
			for (int row = 0; row < rowCount1; ++row)
			{
				r.x = 0;
				r.width = BUTTON_WIDTH;
                if (CheckInsideScroll())
                {
					if (GUI.Button(r, LABEL_CACHE_1))
					{
						Selection.activeObject = null;
						Object obj = AssetDatabase.LoadAssetAtPath<Object>(logText[row]);
						Selection.activeObject = obj;
					}
				}
				r.x += BUTTON_WIDTH + PAD;

				r.width = LABEL_WIDTH;
				if (CheckInsideScroll())
				{
					GUI.Label(r, logText[row], textStyle);
				}
				r.y += HEIGHT + PAD;
			}
			r.x = 0;
			r.width = LABEL_WIDTH;
			if (CheckInsideScroll())
			{
				GUI.Label(r, LABLE_CACHE_3);
			}
			r.y += HEIGHT + PAD;

			for (int row = 0; row < rowCount2; ++row)
			{
				r.x = 0;
				r.width = BUTTON_WIDTH;
				if (CheckInsideScroll())
				{
					if (GUI.Button(r, LABEL_CACHE_1))
					{
						Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(logText2[row]);
					}
				}
				r.x += BUTTON_WIDTH + PAD;
				if (CheckInsideScroll())
				{
					if (GUI.Button(r, LABEL_CACHE_1))
					{
						Object obj = AssetDatabase.LoadAssetAtPath<Object>(logText2[row]);
						Selection.activeObject = obj;
						CheckSingleAsset.Check(CheckDependency.assetFiles, obj);
					}
				}
				r.x += BUTTON_WIDTH + PAD;
				r.width = LABEL_WIDTH;
				if (CheckInsideScroll())
				{
					GUI.Label(r, logText2[row], textStyle);
				}
				r.y += HEIGHT + PAD;
			}
			GUI.EndScrollView();
		}
		private static bool CheckInsideScroll()
        {
			return r.x + r.width > scrollPosition.x
				&& r.x < scrollPosition.x + scrollWidth
				&& r.y + r.height > scrollPosition.y
				&& r.y < scrollPosition.y + scrollHeight;
		}
		
		public static void CheckAsset(string[] searchFiles, string[] resourceFiles)
		{
			Hashtable tableDep = new Hashtable ();
			for (int i = 0; i < searchFiles.Length; i++) {
				string subPath = searchFiles[i].Remove(0, Application.dataPath.Length - 6);
				subPath = subPath.Replace("\\", "/");
				string[] deps = AssetDatabase.GetDependencies(subPath, true);
				foreach (var v in deps)
				{
					if (!tableDep.ContainsKey(v) && v != subPath)
					{
						tableDep.Add(v, true);
					}
				}
			}
			logText.Clear();
			logText2.Clear();
			for (int i = 0; i < resourceFiles.Length; i++)
			{
				string subPath = resourceFiles[i].Remove(0, Application.dataPath.Length - 6);
				subPath = subPath.Replace("\\", "/");
				if(tableDep[subPath] == null)
				{
					string extension = Path.GetExtension(subPath);
					if (!ignoreList.Contains(extension))
					{
						logText.Add(subPath);
					}
				}
				else
				{
					string extension = Path.GetExtension(subPath);
					if (!ignoreList.Contains(extension))
					{
						logText2.Add(subPath);
					}
				}
			}
			logText.Sort(EditorUtility.NaturalCompare);
			logText2.Sort(EditorUtility.NaturalCompare);
			rowCount1 = logText.Count;
			rowCount2 = logText2.Count;
			scrollHeight = (rowCount1 + rowCount2 + 2) * (HEIGHT + PAD);
			OpenWindow();
		}
	}

}
