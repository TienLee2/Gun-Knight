#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using VladislavTsurikov.Editor;

namespace MegaWorld
{
    [Serializable]
    public class FailureSettingsEditor 
    {
        public bool failureSettingsFoldout = true;

        public void OnGUI(FailureSettings failureSettings)
        {
            FailureSettingsGUI(failureSettings);
        }

        public void FailureSettingsGUI(FailureSettings failureSettings)
		{
			failureSettingsFoldout = CustomEditorGUI.Foldout(failureSettingsFoldout, "Failure Settings");

			if(failureSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				failureSettings.Enable = CustomEditorGUI.Toggle(enable, failureSettings.Enable);
				
				if(failureSettings.Enable)
				{
					failureSettings.FailureRate = CustomEditorGUI.Slider(failureRate, failureSettings.FailureRate, 0f, 100f);
					
					CustomEditorGUI.CustomGradient(failureRateFromFitness, failureSettings.FailureRateFromFitness);
				}

				EditorGUI.indentLevel--;
			}			
		}

		[NonSerialized]
		private GUIContent enable = new GUIContent("Enable", "If disabled, then these settings will not affect spawn.");

		[NonSerialized]
		private GUIContent failureRate = new GUIContent("Failure Rate (%)", "The larger this value, the less likely it is to spawn an object.");

		[NonSerialized]
		private GUIContent failureRateFromFitness = new GUIContent("Failure Rate From Fitness", "You can click on the Gradient to edit at which Fitness value the probability of spawning will be. The default will be if the fitness value is 0.5, then the probability of spawn will be 50%. \n* Fitness is the value the tool gets from the filters.");

    }
}
#endif