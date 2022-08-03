#if UNITY_EDITOR
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine.Experimental.Rendering;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;

namespace MegaWorld
{
    public static class VisualisationUtility 
    {
        public static void DrawMaskFilterVisualization(FilterStack maskFilterStack, PainterVariables painterVariables, float multiplyAlpha = 1)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            if (maskFilterStack.Filters.Count > 0)
            {
                FilterContext filterContext = new FilterContext(painterVariables);
                FilterMaskOperation.UpdateFilterContext(ref filterContext, maskFilterStack, painterVariables);

                DrawMaskFilter(filterContext, painterVariables, multiplyAlpha);

                filterContext.DisposeUnmanagedMemory();
            }
            else
            {
                DrawAreaPreview(painterVariables);
            }
        }

        public static void DrawMaskFilter(FilterContext filterContext, PainterVariables painterVariables, float multiplyAlpha = 1)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

            if(MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.EnableDefaultPreviewMaterial)
            {
                terrainPainterRenderHelper.RenderAreaPreview(filterContext.HeightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
            }

            VisualisationUtility.DrawSpawnerShaderVisualisation(terrainPainterRenderHelper, filterContext.HeightContext, filterContext, multiplyAlpha);
        }

        public static void DrawAreaPreview(PainterVariables painterVariables)
        {
            if(painterVariables.terrainUnderCursor == null)
            {
                return;
            }

            TerrainPainterRenderHelper terrainPainterRenderHelper = new TerrainPainterRenderHelper(painterVariables);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();

            if(heightContext == null)
            {
                return;
            }

            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);

            TerrainPaintUtility.ReleaseContextResources(heightContext);
        }

        public static void DrawSpawnerShaderVisualisation(TerrainPainterRenderHelper terrainPainterRenderHelper, PaintContext heightContext, FilterContext filterContext, float multiplyAlpha = 1)
    	{
            Texture brushTexture = terrainPainterRenderHelper.PainterVariables.Mask;

            Material brushMaterial = FilterUtility.GetBrushPreviewMaterial();
            RenderTexture tmpRT = RenderTexture.active;

            RenderTexture filterMaskRT = filterContext.GetFilterMaskRT();

            if(filterMaskRT != null)
            {                
                //Composite the brush texture onto the filter stack result
                RenderTexture compRT = RenderTexture.GetTemporary(filterMaskRT.descriptor);
    			Material blendMat = FilterUtility.GetBlendMaterial();
    			blendMat.SetTexture("_BlendTex", brushTexture);
    			blendMat.SetVector("_BlendParams", new Vector4(0.0f, 0.0f, -(terrainPainterRenderHelper.PainterVariables.Rotation * Mathf.Deg2Rad), 0.0f));
    			TerrainPaintUtility.SetupTerrainToolMaterialProperties(heightContext, terrainPainterRenderHelper.BrushTransform, blendMat);
    			Graphics.Blit(filterMaskRT, compRT, blendMat, 0);

    			RenderTexture.active = tmpRT;

                BrushTransform identityBrushTransform = TerrainPaintUtility.CalculateBrushTransform(terrainPainterRenderHelper.PainterVariables.terrainUnderCursor, terrainPainterRenderHelper.PainterVariables.RaycastInfo.hitInfo.textureCoord, terrainPainterRenderHelper.PainterVariables.Size, 0.0f);

                brushMaterial.SetColor("_Color", MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.Color);
                brushMaterial.SetInt("_EnableBrushStripe", MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.EnableStripe == true ? 1 : 0);      
                brushMaterial.SetInt("_ColorSpace", (int)MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.ColorSpace);   
                brushMaterial.SetInt("_AlphaVisualisationType", (int)MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.AlphaVisualisationType);   
                brushMaterial.SetFloat("_Alpha", MegaWorldPath.AdvancedSettings.VisualisationSettings.MaskFiltersSettings.CustomAlpha * multiplyAlpha);   

                TerrainPaintUtility.SetupTerrainToolMaterialProperties(heightContext, identityBrushTransform, brushMaterial);
    			TerrainPaintUtilityEditor.DrawBrushPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, compRT, identityBrushTransform, brushMaterial, 0);
    			RenderTexture.ReleaseTemporary(compRT);
            }
    	}

        public static void DrawSimpleFilter(Type type, Vector3 originPoint, PainterVariables painterVariables, SimpleFilterSettings filterSettings)
        {
            if(!MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.EnableSpawnVisualization)
            {
                return;
            }

            float stepIncrement = painterVariables.Size / ((float)MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.VisualiserResolution - 1f);

            int x, z;
            Vector3 position = Vector3.zero;
            position.y = painterVariables.RaycastInfo.hitInfo.point.y;

            float halfSpawnRange = painterVariables.Radius;

            Bounds originBoundsSize = new Bounds(originPoint, new Vector3(painterVariables.Size, painterVariables.Size, painterVariables.Size));
            Bounds offsetBoundsSize = new Bounds(originPoint, new Vector3(halfSpawnRange * 2, halfSpawnRange * 2, halfSpawnRange * 2));

            Vector3 maxPosition = originPoint + (Vector3.one * halfSpawnRange);
            Vector3 minExtents = Vector3.zero;

            float step = painterVariables.Size  / ((float)MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.VisualiserResolution - 1f);

            Vector3 localPoint = position;
            Vector3 localNormal = Vector3.up;

            for (x = 0, position.x = originPoint.x - halfSpawnRange; position.x < maxPosition.x; x++, position.x += step)
            {
                for (z = 0, position.z = originPoint.z - halfSpawnRange; position.z < maxPosition.z; z++, position.z += step)
                {
                    float fitness = 0;
                    float alpha = 1;

                    Vector2 normalizedPoint = Utility.GetNormalizedCheckPoint(originPoint, position, painterVariables.Radius);// GetNormalizedCheckPoint(originPoint, position, painterVariables.Size);

                    float brushStrength = painterVariables.GetAlphaRaw(normalizedPoint);

                    fitness = GetFitnessForSpawnVisualisation(type, filterSettings, position, out localPoint, out localNormal);

                    fitness *= brushStrength;

                    alpha = MegaWorldPath.AdvancedSettings.VisualisationSettings.SimpleFilterSettings.Alpha;

                    DrawHandles.DrawSpawnVisualizerPixel(new SpawnVisualizerPixel(localPoint, fitness, alpha), stepIncrement);
                }
            }
        }

        public static void DrawCircleHandles(float size, RaycastInfo hit)
        {   
            float radius = size / 2;
            if(MegaWorldPath.AdvancedSettings.VisualisationSettings.BrushHandlesSettings.DrawSolidDisc == true)
            {
                Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
                Handles.DrawSolidDisc(hit.hitInfo.point, hit.hitInfo.normal, radius);
            }

            switch (MegaWorldPath.AdvancedSettings.VisualisationSettings.BrushHandlesSettings.BrushHandlesType)
    		{
    			case BrushHandlesType.Circle:
    			{
                    DrawCircle(size, hit);
    				break;
    			}
    			case BrushHandlesType.SphereAndCircle:
    			{
                    DrawSphere(size, hit);
    				break;
    			}
    		}
        }

        public static void DrawCircle(float size, RaycastInfo hit)
        {
            Matrix4x4 localTransform = Matrix4x4.TRS(hit.hitInfo.point, Quaternion.LookRotation(hit.hitInfo.normal), new Vector3(size, size, size));

            BrushHandlesSettings brushHandlesSettings  = MegaWorldPath.AdvancedSettings.VisualisationSettings.BrushHandlesSettings;

            Color color = brushHandlesSettings.CircleColor;

            float thickness = brushHandlesSettings.CirclePixelWidth;
            VladislavTsurikov.DrawHandles.DrawCircleWithoutZTest(localTransform, Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.right), Vector3.one), color, thickness);
        }

        public static void DrawSphere(float size, RaycastInfo hit)
        {
            Matrix4x4 localTransform = Matrix4x4.TRS(hit.hitInfo.point, Quaternion.LookRotation(Vector3.up), new Vector3(size, size, size));

            BrushHandlesSettings brushHandlesSettings  = MegaWorldPath.AdvancedSettings.VisualisationSettings.BrushHandlesSettings;

            float thickness = MegaWorldPath.AdvancedSettings.VisualisationSettings.BrushHandlesSettings.SpherePixelWidth;

            Color color = brushHandlesSettings.SphereColor;

            DrawCircle(size, hit);
            VladislavTsurikov.DrawHandles.DrawSphere(localTransform, color, thickness);
        }

        public static bool IsActiveSimpleFilter(SelectedTypeVariables selectedVariables)
        {
            foreach (Type type in selectedVariables.SelectedTypeList)
            {
                if(type.FilterType == FilterType.SimpleFilter)
                {
                    return true;
                }
            }

            return false;
        }

        public static float GetFitnessForSpawnVisualisation(Type type, SimpleFilterSettings filterSettings, Vector3 checkPoint, out Vector3 point, out Vector3 normal)
        {                
            point = checkPoint;
            normal = Vector3.up;

            RaycastHit hitInfo;

            if (Physics.Raycast(new Ray(new Vector3(checkPoint.x, checkPoint.y + MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.SpawnCheckOffset, checkPoint.z), Vector3.down), 
                out hitInfo, MegaWorldPath.AdvancedSettings.EditorSettings.raycastSettings.MaxRayDistance, type.GetCurrentPaintLayers()))
    	    {
                point = hitInfo.point;
                normal = hitInfo.normal;

                float viability = filterSettings.GetFitness(hitInfo.point, hitInfo.normal);

                return viability;
            }

            return 0;
        }
    }
}
#endif