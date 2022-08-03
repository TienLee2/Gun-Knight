using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace VladislavTsurikov
{
    public enum StyleName
    {
    	Bar,
        ActiveBar,
    	LabelButton,
        LabelButtonClick,
        LabelHelp,
    	LabelFoldout,
    	LabelButtonSCenter,
    	LabelSLeft,
    	LabelTextForIcon,
    	LabelTextForSelectedArea,
        LabelText,
    	IconButtonMinus,
    	IconButtonPlus,
    	IconButtonOk,
    	IconButtonCancel,
        BackgroundStyle,
        Box
    }

    public enum ButtonSize
    {
        DragAndDropButton,
        ToolButton,
        ClickButton,
        General
    }

    public enum ButtonStyle
    {
        Rename,
        Add,
        Remove,
        General,
        ButtonClick,
        Foldout
    }
    
    [Serializable]
    public class GeneralPath : ScriptableObject
    {
        private const string ASSETS_PATH = "Assets/";

        public static string pathToVladislavTsurikovFolder = BasePath;

        private const string GUISkinFileName = "GUISkin.guiskin";
        public static string FoldoutRightFileName = "FoldoutRight.png";
        public static string FoldoutDownFileName = "FoldoutDown.png";
        
        public static string GUISkin = "GUISkin";
        public static string Images = "Images";
        public static string Foldout = "Foldout";
        public static string Resources = "Resources";

        public static string pathToGUISkin = CombinePath(pathToVladislavTsurikovFolder, GUISkin);
        public static string pathToImages = CombinePath(pathToGUISkin, Images);
        public static string pathToFoldout = CombinePath(pathToImages, Foldout);

        public static string foldoutRightPath = CombinePath(pathToFoldout, FoldoutRightFileName);
    	public static string foldoutDownPath = CombinePath(pathToFoldout, FoldoutDownFileName);

        public static readonly string skinPath = CombinePath(pathToGUISkin, GUISkinFileName);

        public static string pathToResources = CombinePath("Assets", Resources);

        private static string s_basePath;

        public static string BasePath
        { 
            get
            {
#if UNITY_EDITOR

                if (!string.IsNullOrEmpty(s_basePath)) return s_basePath;
                var obj = CreateInstance<GeneralPath>();
                UnityEditor.MonoScript s = UnityEditor.MonoScript.FromScriptableObject(obj);
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
                DestroyImmediate(obj);
                var fileInfo = new FileInfo(assetPath);
                UnityEngine.Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
                UnityEngine.Debug.Assert(fileInfo.Directory.Parent != null, "fileInfo.Directory.Parent != null");
                DirectoryInfo baseDir = fileInfo.Directory.Parent;
                UnityEngine.Debug.Assert(baseDir != null, "baseDir != null");
                Assert.AreEqual("Mega World", baseDir.Name);
                string baseDirPath = ReplaceBackslashesWithForwardSlashes(baseDir.ToString());
                int index = baseDirPath.LastIndexOf(ASSETS_PATH, StringComparison.Ordinal);
                Assert.IsTrue(index >= 0);
                baseDirPath = baseDirPath.Substring(index);
                s_basePath = baseDirPath;

                pathToVladislavTsurikovFolder = s_basePath;

                return s_basePath;

#else
                return "";
#endif
            }
        }

        public static string ReplaceBackslashesWithForwardSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        public static string CombinePath(string path1, string path2)
        {
            return path1 + "/" + path2;
        }
    }
}