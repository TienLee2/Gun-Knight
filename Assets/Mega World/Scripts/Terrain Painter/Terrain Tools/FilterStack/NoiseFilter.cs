using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace MegaWorld
{
    [Serializable]
    public class NoiseFilter : Filter
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public NoiseSettings NoiseSettings = new NoiseSettings();

        public override void Eval(FilterContext fc, int index)
        {
            CreateNoiseSettingsIfNecessary();

            Vector3 brushPosWS = fc.BrushPos;
            float brushSize = fc.PainterVariables.Size;
            float brushRotation = fc.PainterVariables.Rotation;

            // TODO(wyatt): remove magic number and tie it into NoiseSettingsGUI preview size somehow
            float previewSize = 1 / 512f;

            // get proper noise material from current noise settings
            Material mat = NoiseUtils.GetDefaultBlitMaterial(NoiseSettings);

            // setup the noise material with values in noise settings
            NoiseSettings.SetupMaterial( mat );

            // convert brushRotation to radians
            brushRotation *= Mathf.PI / 180;

            // change pos and scale so they match the noiseSettings preview
            bool isWorldSpace = true;
            //brushSize = isWorldSpace ? brushSize * previewSize : 1;
            brushSize = brushSize * previewSize;
            brushPosWS = isWorldSpace ? brushPosWS * previewSize : Vector3.zero;

            // // override noise transform
            Quaternion rotQ             = Quaternion.AngleAxis( -brushRotation, Vector3.up );
            Matrix4x4 translation       = Matrix4x4.Translate( brushPosWS );
            Matrix4x4 rotation          = Matrix4x4.Rotate( rotQ );
            Matrix4x4 scale             = Matrix4x4.Scale( Vector3.one * brushSize );
            Matrix4x4 noiseToWorld      = translation * scale;

            mat.SetMatrix(NoiseSettings.ShaderStrings.Transform, NoiseSettings.trs * noiseToWorld);

            int pass = NoiseUtils.kNumBlitPasses * NoiseLib.GetNoiseIndex( NoiseSettings.DomainSettings.NoiseTypeName );

            RenderTextureDescriptor desc = new RenderTextureDescriptor(fc.DestinationRenderTexture.width, fc.DestinationRenderTexture.height, RenderTextureFormat.RFloat);
            RenderTexture rt = RenderTexture.GetTemporary(desc);

            Graphics.Blit(fc.SourceRenderTexture, rt, mat, pass);

            Material blendMat = FilterUtility.blendModesMaterial;
            
            if(index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", fc.SourceRenderTexture);
            blendMat.SetTexture("_BlendTex", rt);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(rt);
        }

#if UNITY_EDITOR
        private NoiseSettingsGUI noiseSettingsGUI;
        private NoiseSettingsGUI NoiseSettingsGUI
        {
            get
            {
                if(noiseSettingsGUI == null)
                {
                    noiseSettingsGUI = new NoiseSettingsGUI(NoiseSettings);
                }

                return noiseSettingsGUI;
            }
        }

        public override void DoGUI(Rect rect, int index) 
        {
            if(index != 0)
            {
                BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            NoiseSettingsGUI.OnGUI(rect);
        }

        public override float GetElementHeight(int index) 
        {
            CreateNoiseSettingsIfNecessary();
            
            float height = EditorGUIUtility.singleLineHeight;

            if (NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + 40f;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            if (NoiseSettings.ShowNoiseTypeSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 15;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override string GetDisplayName()
        {
            return "Noise";
        }

        public override string GetToolTip()
        {
            return "Applies noise to the brush mask based on the Noise Settings associated with this filter. To edit the noise, press the \"Edit\" or \"E\" button";
        }
#endif

        private void CreateNoiseSettingsIfNecessary()
        {
            if(NoiseSettings == null)
            {
                NoiseSettings = new NoiseSettings();
            }
        }
    }
}