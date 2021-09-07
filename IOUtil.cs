using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DependencyChecker
{
    public class IOUtil
    {
        public static string GetRelativeAssetPath(string fullPath)
        {
            fullPath = GetRightFormatPath(fullPath);
            int index = fullPath.IndexOf("Assets");
            string relativePath = fullPath.Substring(index);
            return relativePath;
        }

        public static string GetRightFormatPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static List<string> fileList = new List<string>();
        public static string[] GetFiles(string fullPath, bool isFullName = true)
        {
            fileList.Clear();
            DirectoryInfo dirs = new DirectoryInfo(fullPath);
            DirectoryInfo[] dir = dirs.GetDirectories();
            FileInfo[] file = dirs.GetFiles();
            int dircount = dir.Count();
            int filecount = file.Count();
            for (int i = 0; i < dircount; i++)
            {
                string pathNode = fullPath + "/" + dir[i].Name;
                GetMultiFile(pathNode, isFullName);
            }

            for (int j = 0; j < filecount; j++)
            {
                if (isFullName)
                {
                    fileList.Add(file[j].FullName);
                }
                else
                {
                    fileList.Add(file[j].Name);
                }
            }
            return fileList.ToArray();
        }

        private static bool GetMultiFile(string path, bool isFullName = true)
        {
            if (Directory.Exists(path) == false)
            {
                Debug.Log("Directory.Exists(path) == false");
                return false;
            }
            DirectoryInfo dirs = new DirectoryInfo(path);
            DirectoryInfo[] dir = dirs.GetDirectories();
            FileInfo[] file = dirs.GetFiles();
            int dircount = dir.Count();
            int filecount = file.Count();
            int sumcount = dircount + filecount;

            if (sumcount == 0)
            { return false; }

            for (int j = 0; j < dircount; j++)
            {
                string pathNode = path + "/" + dir[j].Name;
                GetMultiFile(pathNode, isFullName);
            }

            for (int j = 0; j < filecount; j++)
            {
                if (isFullName)
                {
                    fileList.Add(file[j].FullName);
                }
                else
                {
                    fileList.Add(file[j].Name);
                }
            }
            return true;
        }

        public static List<string> GetAllAssetPath(string selectAssetPath, string extension)
        {
            List<string> assetPaths = new List<string>();
            var folderpath = Path.GetFullPath(selectAssetPath);
            var files = GetFiles(folderpath);
            for (int i = 0; i < files.Length; i++)
            {
                string extensionName = Path.GetExtension(files[i]);
                if (extensionName == extension)
                {
                    assetPaths.Add(GetRelativeAssetPath(files[i]));
                }
            }
            return assetPaths;
        }
    }

}
