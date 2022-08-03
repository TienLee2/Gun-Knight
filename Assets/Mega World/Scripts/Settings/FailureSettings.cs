using UnityEngine;
using System;
using VladislavTsurikov;

namespace MegaWorld
{
    [Serializable]
    public class FailureSettings
    {
        public bool Enable = true;
        public CustomGradient FailureRateFromFitness = new CustomGradient();

        [Range (0, 100)]
        public float FailureRate = 80f;

        #if UNITY_EDITOR
        public FailureSettingsEditor FailureSettingsEditor = new FailureSettingsEditor();

        public void OnGUI()
        {
            FailureSettingsEditor.OnGUI(this);
        }
        #endif

        public FailureSettings()
        {

        }

        public FailureSettings(FailureSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(FailureSettings other)
        {            
            FailureRateFromFitness = new CustomGradient(other.FailureRateFromFitness);
            FailureRate = other.FailureRate;
        }

        public bool CheckFailureRate(ref float fitness)
        {
            fitness = FailureRateFromFitness.Evaluate(fitness).r;

            if(Enable)
            {
                if (UnityEngine.Random.Range(0f, 1f) < (1 - fitness))
                {
                    return false;
                }
                else if (UnityEngine.Random.Range(0f, 100f) < FailureRate)
                {
                    return false;
                }
            }

            return true;
        }
    }
}