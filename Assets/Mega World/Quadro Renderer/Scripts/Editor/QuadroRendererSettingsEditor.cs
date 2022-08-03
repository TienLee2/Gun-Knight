using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov;
using VladislavTsurikov.Editor;

namespace QuadroRendererSystem
{
    [CustomEditor(typeof(QuadroRendererSettings))]
    public class QuadroRendererSettingsEditor : Editor
    {
        private QuadroRendererSettings settings;

        private void OnEnable()
        {
            settings = (QuadroRendererSettings)target;
        }

        public override void OnInspectorGUI()
        {
            CustomEditorGUI.isInspector = true;
            QuadroRendererSettingsEditor.OnGUI(settings);
        }

        public static void OnGUI(QuadroRendererSettings settings)
        {
            settings.AutoShaderConversion = CustomEditorGUI.Toggle(new GUIContent("Auto Shader Conversion"), settings.AutoShaderConversion);
            settings.ShowRenderModelData = CustomEditorGUI.Toggle(new GUIContent("Show Render Model Data"), settings.ShowRenderModelData);
            settings.RenderDirectToCamera = CustomEditorGUI.Toggle(new GUIContent("Render Direct To Camera"), settings.RenderDirectToCamera);
            settings.RenderSceneCameraInPlayMode = CustomEditorGUI.Toggle(new GUIContent("Render Scene Camera in PlayMode"), settings.RenderSceneCameraInPlayMode);

            EditorGUI.BeginDisabledGroup(true);
            CustomEditorGUI.Toggle(new GUIContent("HDRP Loaded"), settings.IsHDRP);
            CustomEditorGUI.Toggle(new GUIContent("LWRP Loaded"), settings.IsLWRP);
            CustomEditorGUI.Toggle(new GUIContent("URP Loaded"), settings.IsURP);
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUI.GetCurrentSpace());
				if(CustomEditorGUI.ClickButton("Find Current Render Pipeline", ButtonStyle.Add))
				{
					settings.PackagesLoaded = false;
                    QuadroRendererDefines.FindCurrentRenderPipeline();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/Quadro Renderer", SettingsScope.User)
            {
                label = "Quadro Renderer",
                guiHandler = (searchContext) =>
                {
                    OnGUI(QuadroRendererConstants.QuadroRendererSettings);
                },
                keywords = new HashSet<string>(new[] { "Quadro Renderer" })
            };

            return provider;
        }
    }
}