using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace MegaWorld.BrushModify
{
    [Serializable]
    public class ModifyTransformComponentsStack 
    {
        [SerializeField]
        public List<ModifyTransformComponent> TransformComponents = new List<ModifyTransformComponent>();
        private static ModifyTransformComponent s_ClipboardContent;

        public void Clear()
        {
            TransformComponents.Clear();
        }

        public void CopyTransformComponentsStack(ModifyTransformComponentsStack filterStack, Type type)
        {
#if UNITY_EDITOR
            TransformComponents.Clear();

            foreach (var item in filterStack.TransformComponents)
            {
                var effect = CreateNewType(item.GetType());
                TransformComponents.Add(effect);

                CopySettings(item);
                PasteSettings(TransformComponents[TransformComponents.Count - 1]);

                AssetDatabase.AddObjectToAsset(effect, type);   
            }

            AssetDatabase.SaveAssets();
#endif
        }

        static void CopySettings(ModifyTransformComponent target)
        {
#if UNITY_EDITOR
            if (s_ClipboardContent != null)
            {
                RuntimeUtilities.Destroy(s_ClipboardContent);
                s_ClipboardContent = null;
            }

            s_ClipboardContent = (ModifyTransformComponent)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_ClipboardContent);
#endif
        }

        static void PasteSettings(ModifyTransformComponent target)
        {
#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(s_ClipboardContent, target);
#endif
        }

        private ModifyTransformComponent CreateNewType(System.Type type)
        {
            var filter = (ModifyTransformComponent)ScriptableObject.CreateInstance(type);
            filter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            filter.name = type.Name;
            return filter;
        }

        public bool HasSettings(System.Type type)
        {
            foreach (var setting in TransformComponents)
            {
                if (setting.GetType() == type)
                    return true;
            }

            return false;
        }
    }
}
