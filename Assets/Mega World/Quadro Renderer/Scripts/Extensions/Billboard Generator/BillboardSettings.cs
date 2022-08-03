using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public enum BillboardQuality
    {
        Low = 0,
        Mid = 1,
        High = 2,
        VeryHigh = 3
    }
    
    [Serializable]
    public class BillboardSettings
    {
        public bool UseHueVariation;
        public bool NormalInvert = false;
        public Color TranslucencyColor = Color.black;
		public float ShadowStrength = 0.7f;
        public Color HealthyColor = Color.white;
        public Color DryColor = new Color(0.85f, 0.85f, 0.75f, 1);
        public float ColorNoiseSpread = 50;

        public bool UseGeneratedBillboard = false;
        public BillboardGenerationActions BillboardGeneratorAction = BillboardGenerationActions.ReplaceLastLOD;
        public BillboardQuality BillboardQuality = BillboardQuality.Mid;
        public int AtlasResolution = 8192;
        public int FrameCount = 16;
        public Color Color = new Color(0.83f, 0.83f, 0.83f, 1);
        public float CutOffOverride = 0.5f;
        public bool BillboardFaceCamPos = false;

        public string[] TEXT_BillboardQualityOptions = { "Low (1024)", "Mid (2048)", "High (4096)", "Very High (8192)" };
        [Range(0.0f, 1.0f)]
        public float BillboardBrightness = 1f;

        public Texture2D AlbedoAtlasTexture;
        public Texture2D NormalAtlasTexture;

        public float QuadSize;
        public float YPivotOffset;

        public bool DeleteBillboardTextures(QuadroPrototype selectedPrototype)
        {
            bool billboardsDeleted = false;
#if UNITY_EDITOR
            if (selectedPrototype.BillboardSettings != null && selectedPrototype.BillboardSettings.AlbedoAtlasTexture != null)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedPrototype.BillboardSettings.AlbedoAtlasTexture));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedPrototype.BillboardSettings.NormalAtlasTexture));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                billboardsDeleted = true;
            }
#endif

            return billboardsDeleted;
        }

        public BillboardSettings()
        {

        }

        public BillboardSettings(BillboardSettings other)
        {
            CopyFrom(other);
        }

        public void CopyFrom(BillboardSettings other)
        {            
            UseHueVariation = other.UseHueVariation;
            NormalInvert = other.UseHueVariation;
            TranslucencyColor = other.TranslucencyColor;
		    ShadowStrength = other.ShadowStrength;
            HealthyColor = other.HealthyColor;
            DryColor = other.DryColor;
            ColorNoiseSpread = other.ColorNoiseSpread;
            UseGeneratedBillboard = other.UseGeneratedBillboard;
            BillboardGeneratorAction = other.BillboardGeneratorAction;
            BillboardQuality = other.BillboardQuality;
            AtlasResolution = other.AtlasResolution;
            FrameCount = other.FrameCount;
            CutOffOverride = other.CutOffOverride;
            Color = other.Color;
            BillboardBrightness = other.BillboardBrightness;
            BillboardFaceCamPos = other.BillboardFaceCamPos;
        }
    }
}