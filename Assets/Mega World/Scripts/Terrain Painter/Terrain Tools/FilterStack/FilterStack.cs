using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace MegaWorld
{
    public enum BlendMode
    {
        Multiply,
        Add,
    }

    public enum ColorSpaceForBrushMaskFilter
    {
        СustomColor,
        Colorful,
        Heightmap
    }

    [Serializable]
    public class FilterStack
    {
        public List<Filter> Filters = new List<Filter>();
        private static Filter s_ClipboardContent;

        public void Eval(FilterContext fc)
        {
            int count = Filters.Count;

            RenderTexture prev = RenderTexture.active;

            RenderTexture[] rts = new RenderTexture[2];

            int srcIndex = 0;
            int destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);
            rts[1] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);

            rts[0].enableRandomWrite = true;
            rts[1].enableRandomWrite = true;

            Graphics.Blit(Texture2D.whiteTexture, rts[0]);
            Graphics.Blit(Texture2D.blackTexture, rts[1]); //don't remove this! needed for compute shaders to work correctly.

            for( int i = 0; i < count; ++i )
            {
                if( Filters[ i ].Enabled )
                {
                    fc.SourceRenderTexture = rts[srcIndex];
                    fc.DestinationRenderTexture = rts[destIndex];

                    Filters[ i ].Eval(fc, i);

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex = destIndex - srcIndex;
                }
            }

            Graphics.Blit(rts[srcIndex], fc.Output);//fc.destinationRenderTexture);

            RenderTexture.ReleaseTemporary(rts[0]);
            RenderTexture.ReleaseTemporary(rts[1]);

            RenderTexture.active = prev;
        }

        public void Clear()
        {
            Filters.Clear();
        }

        public void CopyFilterStack(FilterStack filterStack, Type type)
        {
#if UNITY_EDITOR
            Filters.Clear();

            foreach (var item in filterStack.Filters)
            {
                var effect = CreateNewType(item.GetType());
                Filters.Add(effect);

                CopySettings(item);
                PasteSettings(Filters[Filters.Count - 1]);

                AssetDatabase.AddObjectToAsset(effect, type);   
            }

            AssetDatabase.SaveAssets();
#endif
        }

        static void CopySettings(Filter target)
        {
#if UNITY_EDITOR
            if (s_ClipboardContent != null)
            {
                RuntimeUtilities.Destroy(s_ClipboardContent);
                s_ClipboardContent = null;
            }

            s_ClipboardContent = (Filter)ScriptableObject.CreateInstance(target.GetType());
            EditorUtility.CopySerializedIfDifferent(target, s_ClipboardContent);
#endif
        }

        static void PasteSettings(Filter target)
        {
#if UNITY_EDITOR
            EditorUtility.CopySerializedIfDifferent(s_ClipboardContent, target);
#endif
        }

        private Filter CreateNewType(System.Type type)
        {
            var filter = (Filter)ScriptableObject.CreateInstance(type);
            filter.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            filter.name = type.Name;
            return filter;
        }
    }
}