using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;

namespace DependencyChecker
{
	public class GetAllDependency : EditorWindow 
	{ 
		private const float WINDOW_WIDTH = 800;
		private const float WINDOW_HEIGHTTH = 800;
		private static float scrollHeight = 0;
		private const float scrollWidth = 1000;
		private const float PAD = 1f;
		private const float HEIGHT = 20;
		private const float BUTTON_WIDTH = 20;
		private const float BUTTON_WIDTH_2 = 40;

		private const float ASSET_PATH_WIDTH = 800;
		private const string LABEL_CACHE_1 = "";
		private GUIStyle textStyle;
		private Vector2 scrollPosition = Vector2.zero;
		private Rect r = new Rect(0, 0, BUTTON_WIDTH, HEIGHT);
		private List<string> abNames = new List<string>();
		private List<string> assetPaths = new List<string>();
		private List<string> allAbNames = new List<string>();
		private bool divideByAbNames = false;
		private int pageIndex;
		private List<List<string>> pages = new List<List<string>>();
		private List<string> pageFiles = new List<string>();
		private List<string> pageAbNames = new List<string>();

		private float abNameWidth = 100;
		private int nMaxLength = 0;

		private static bool showAbName = false;

		void OnGUI ()
		{
			r.x = PAD;
			r.y = PAD;

			r.width = 300;
			showAbName = GUI.Toggle(r, showAbName, "Show Assetbundle Name");
			r.y += r.height + PAD;

			r.x = PAD;
			r.height = BUTTON_WIDTH;
			if (divideByAbNames)
            {
				r.width = BUTTON_WIDTH_2;
				if (GUI.Button(r, "all"))
				{
					divideByAbNames = false;
					abNameWidth = GetTextWidth(nMaxLength);
				}
				r.x += r.width + PAD;
			}
            else
            {
				r.width = BUTTON_WIDTH_2;
				if (GUI.Button(r, "div"))
				{
					onDivideButtonClick();
				}
				r.x += r.width + PAD;
			}
			
            if (divideByAbNames)
            {
				for (int i = 0; i < allAbNames.Count; i++)
                {
					r.height = BUTTON_WIDTH;
					if(i == pageIndex)
					{
						r.height = BUTTON_WIDTH * 1.5f;
					}
					string abName = allAbNames[i];
					r.width = GetTextWidth(abName.Length) + 10;
					if (GUI.Button(r, abName))
					{
						pageIndex = i;
						SetPage(pageIndex);
					}
					r.x += r.width + PAD;
				}
				r.y += BUTTON_WIDTH * 1.5f + PAD;
            }
			else
			{
				r.y += r.height + PAD;
			}
			
            if (divideByAbNames)
            {
				ShowAll(pageAbNames, pageFiles);
			}
            else
            {
				ShowAll(abNames, assetPaths);
            }
		}

		private void ShowAll(List<string> abNames, List<string> assetPaths)
        {
			int rowCount = assetPaths.Count;
			scrollHeight = rowCount * (HEIGHT + PAD);
			textStyle = GUI.skin.GetStyle("Label");
			textStyle.alignment = TextAnchor.MiddleLeft;
			Rect viewRect = new Rect(PAD, r.y, WINDOW_WIDTH, WINDOW_HEIGHTTH);
			Rect scrollRect = new Rect(PAD, r.y, scrollWidth, scrollHeight + 40);
			scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, scrollRect, true, true);
			r.x = PAD;
			for (int row = 0; row < rowCount; row++)
			{
				r.x = PAD;
				//Select Asset
				r.height = BUTTON_WIDTH;
				r.width = BUTTON_WIDTH;
				if (CheckInsideScroll(r, viewRect, scrollPosition))
				{
					if (GUI.Button(r, LABEL_CACHE_1))
					{
						Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPaths[row]);
						Selection.activeObject = obj;
					}
				}
				r.x += r.width + PAD;
				//Open Referenced Window
				r.width = BUTTON_WIDTH;
				if (CheckInsideScroll(r, viewRect, scrollPosition))
				{
					if (GUI.Button(r, LABEL_CACHE_1))
					{
						Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPaths[row]);
						Selection.activeObject = obj;
						CheckSingleAsset.Check(CheckDependency.assetFiles, obj);
					}
				}
				r.x += r.width + PAD;
				//Assetbundle Name
				if(!divideByAbNames && showAbName){
					r.width = abNameWidth;
					if (CheckInsideScroll(r, viewRect, scrollPosition))
					{
						GUI.Label(r, abNames[row], textStyle);
					}
					r.x += r.width + PAD;
				}
				//Asset Path
				r.width = ASSET_PATH_WIDTH;
				if (CheckInsideScroll(r, viewRect, scrollPosition))
				{
					GUI.Label(r, assetPaths[row], textStyle);
				}
				r.x += r.width + PAD;

				r.y += HEIGHT + PAD;
			}
			GUI.EndScrollView();
		}

		private void onDivideButtonClick()
        {
			DivideByAbNames();
			SetPage(pageIndex);
			divideByAbNames = true;
		}

		private void SetPage(int pageIndex)
        {
			string abName = allAbNames[pageIndex];
			abNameWidth = GetTextWidth(abName.Length);
			pageFiles = pages[pageIndex];
			pageAbNames.Clear();
			for (int i = 0; i < pages[pageIndex].Count; i++)
			{
				pageAbNames.Add(abName);
			}
		}

		private void DivideByAbNames()
        {
			pageIndex = 0;
			allAbNames.Clear();
			pages.Clear();
			pageFiles.Clear();
			pageAbNames.Clear();
			for (int i = 0; i < assetPaths.Count; i++)
            {
				string abName = abNames[i];
				if(abNames[i] == null)
				{
					abName = " ";
				}
				int index = allAbNames.IndexOf(abName);
				if(index != -1)
				{
					pages[index].Add(assetPaths[i]);
				}
				else
				{
					allAbNames.Add(abName);
					List<string> page = new List<string>();
					page.Add(assetPaths[i]);
					pages.Add(page);
				}
			}
        }
		
		private static bool CheckInsideScroll(Rect r, Rect viewRect, Vector2 scrollPosition)
        {
			return r.x + r.width > scrollPosition.x + viewRect.x
				&& r.x < scrollPosition.x + scrollWidth + viewRect.x
				&& r.y + r.height > scrollPosition.y + viewRect.y
				&& r.y < scrollPosition.y + scrollHeight + viewRect.y;
		}

		private static float GetTextWidth(int lenght)
		{
			return Mathf.Max(lenght * 7, 30);
		}


		[MenuItem("Assets/GetAllDependency", false, CheckAssets.ORDER + 3)]
		private static void onClick()
		{
			GetAllDependency window = (GetAllDependency) EditorWindow.GetWindowWithRect(typeof (GetAllDependency), new Rect(0, 0, WINDOW_WIDTH, WINDOW_HEIGHTTH));
			window.titleContent = new GUIContent("GetAllDependencies");
			window.GetAllDenpendencies();
		}
		
		public void GetAllDenpendencies()
		{
			Object obj = Selection.GetFiltered(typeof(Object), SelectionMode.Assets)[0];
			var path = AssetDatabase.GetAssetPath(obj);
			Selection.activeObject = null;

			string[] searchFiles = IOUtil.GetFiles(path);
			CheckDependency.assetFiles = searchFiles;
			Hashtable dependencies = new Hashtable();
			for(int i = 0;i < searchFiles.Length;i++)
			{
				string subPath = searchFiles[i].Remove(0, Application.dataPath.Length - 6);
				subPath = subPath.Replace("\\", "/");
				var deps = AssetDatabase.GetDependencies(subPath, true);
				foreach (var v in deps)
				{
					if(!dependencies.ContainsKey(v)){
						dependencies.Add(v, true);
					}
				}
			}
			assetPaths.Clear();
			foreach(string k in dependencies.Keys)
			{
				assetPaths.Add(k);
			}
			assetPaths.Sort(EditorUtility.NaturalCompare);
			abNames.Clear();
            int nCount = assetPaths.Count;
			nMaxLength = 0;
            for(int i = 0;i < nCount; i++)
            {
				string abName = AssetImporter.GetAtPath(assetPaths[i]).assetBundleName;
				abNames.Add(abName);
				nMaxLength = Mathf.Max(nMaxLength, abName.Length);
			}
			abNameWidth = GetTextWidth(nMaxLength);
			divideByAbNames = false;
			pages.Clear();
			pageFiles.Clear();
			pageIndex = 0;

		}
	}
}
