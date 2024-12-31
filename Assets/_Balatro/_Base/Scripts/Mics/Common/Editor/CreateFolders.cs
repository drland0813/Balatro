using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace Drland
{
    public class CreateFolders : EditorWindow
    {
        private static string _rootName = "Assets/";
        private static string _projectName = "_" + Application.productName;

        [MenuItem("Drland/Create Default Folders")]
        private static void SetUpFolders()
        {
            CreateAllFolders();
        }

        private static void CreateAllFolders()
        {
            var folders = new List<string>()
            {
                "Animations",
                "Audio",
                "Editor",
                "Materials",
                "Meshes",
                "Prefabs",
                "Scripts",
                "Scenes",
                "Shaders",
                "Textures",
                "UI",
                "Resources"
            };
            if (!Directory.Exists(_rootName + _projectName))
            {
                Directory.CreateDirectory(_rootName + _projectName);
            }
            

            foreach (var folder in folders)
            {
                if (!Directory.Exists(_rootName + _projectName + "/" + folder))
                {
                    Directory.CreateDirectory(_rootName + _projectName + "/" + folder);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
