using System;
using UnityEngine;
using UnityEditor;

namespace MegaWorld.BrushModify
{
    [Serializable]
    public class ModifyTransformComponent : ScriptableObject
    {
        public bool Enabled = true;
        public bool FoldoutGUI = true;
        
        public virtual void SetInstanceData(ref InstanceData spawnInfo, ref BrushModifyTool.ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, Vector3 normal) {}
#if UNITY_EDITOR
        public virtual void DoGUI( Rect rect, int index ) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
        public virtual string GetDisplayName() => "EMPTY_FILTER_NAME";
        public virtual string GetToolTip() => "EMPTY_TOOLTIP";
#endif
    }
}
