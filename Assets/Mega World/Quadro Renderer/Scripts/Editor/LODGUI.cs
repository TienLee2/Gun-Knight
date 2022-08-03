using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuadroRendererSystem
{
    static class LODGUI
    {
        public class GUIStyles
        {
            public readonly GUIStyle LODSliderBGStyle = "LODSliderBG";
            public readonly GUIStyle LODSliderRangeStyle = "LODSliderRange";
            public readonly GUIStyle LODSliderTextStyle = "LODSliderText";
        }

        public class LODGUIInfo
        {
            public Rect ButtonRect;
            public Rect RangeRect;

            public readonly int LODIndex;
            public readonly string LODName;
            public readonly float LODPercentage;

            public LODGUIInfo(int lodLevel, string name, float percentage)
            {
                LODIndex = lodLevel;
                LODName = name;
                LODPercentage = percentage;
            }
        }

        #region Pre Defines

        private static int s_sliderControlId = "LODSliderIDHash".GetHashCode();

        public static readonly Color[] LODColors =
        {
            new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
            new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
            new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
            new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
            new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        public static readonly Color s_culledLODColor = new Color(.4f, 0f, 0f, 1f);
        public static readonly Color s_hiddenLODColor = new Color(.4f, .2f, .2f, .5f);

        private static GUIStyles s_styles;
        public static GUIStyles Styles { get { if (s_styles == null) s_styles = new GUIStyles(); return s_styles; } }

        #endregion

        public static void DrawLODSettingsStack(QuadroPrototype proto, RenderModelInfo renderModelInfo, QuadroRenderer quadroRenderer)
        {
            GUILayout.Space(2);

            #region LODs reset and preparation to draw

            float lodBias = UnityEngine.QualitySettings.lodBias * quadroRenderer.QualitySettings.LodBias * proto.LODSettings.LodBias;

            Rect sliderRect = GUILayoutUtility.GetRect(0, 44, GUILayout.ExpandWidth(true));

            List<LODGUIInfo> infos = CreateLODGUIInfo(renderModelInfo, quadroRenderer, sliderRect, lodBias);

            #endregion

            DrawLODLevelsSlider(GetDistanceMeasures(renderModelInfo.LODs, lodBias), infos, sliderRect, 
                quadroRenderer.QualitySettings.MaxRenderDistance, Mathf.Min(proto.CullingSettings.MaxDistance, 
                quadroRenderer.QualitySettings.MaxRenderDistance));
        }

        public static List<LODGUIInfo> CreateLODGUIInfo(RenderModelInfo renderModelInfo, QuadroRenderer quadroRenderer, Rect sliderRect, float lodBias)
        {
            List<LODGUIInfo> infos = new List<LODGUIInfo>();

            Rect buttonsRect = sliderRect;
            buttonsRect = new Rect(sliderRect.x, sliderRect.y, sliderRect.width, sliderRect.height - 14);

            for (int i = 0; i < renderModelInfo.LODs.Count; i++)
            {
                string name = GetLODName(i, renderModelInfo.LODs.Count);
                
                float percent = 1;
                
                if(i != renderModelInfo.LODs.Count - 1)
                {
                    percent = (renderModelInfo.LODs[i + 1].Distance * lodBias) / quadroRenderer.QualitySettings.MaxRenderDistance;
                }
                
                LODGUIInfo info = new LODGUIInfo(i, name, percent);
                info.ButtonRect = CalcLODButton(buttonsRect, info.LODPercentage);

                float previousPerc = 0f;
                if (i != 0) previousPerc = infos[i - 1].LODPercentage;
                float percentage = info.LODPercentage;

                info.RangeRect = CalcLODRange(buttonsRect, previousPerc, info.LODPercentage);

                infos.Add(info);
            }

            return infos;
        }

        public static void DrawLODLevelsSlider(float[] measure, List<LODGUIInfo> lods, Rect lodRect, float maxDistance, float maxProtoDistance)
        {
            int sliderId = GUIUtility.GetControlID(s_sliderControlId, FocusType.Passive);
            Event evt = Event.current;

            if(evt.GetTypeForControl(sliderId) == EventType.Repaint)
            {
                DrawLODSlider(measure, lodRect, lods, maxDistance, maxProtoDistance);
            }
        }

        /// <summary>
        /// Drawing slider for LOD level
        /// </summary>
        public static void DrawLODSlider(float[] measure, Rect area, IList<LODGUIInfo> lodInfos, float maxDistance, float maxProtoDistance)
        {
            Rect areaA = area;

            areaA = new Rect(area.x, area.y, area.width, area.height - 14);

            // Drawing BG Rectangle
            Styles.LODSliderBGStyle.Draw(areaA, GUIContent.none, false, false, false, false);

            // Drawing LOD rectangle areas
            for (int i = 0; i < lodInfos.Count; i++)
            {
                LODGUIInfo lodInfo = lodInfos[i];

                float startPercent = 0.0f;
                if (i != 0) startPercent = lodInfos[i - 1].LODPercentage;

                DrawLODRange(lodInfo, startPercent, measure[i], maxDistance);
            }

            // Draw culled range as last one
            if (lodInfos == null) return;
            if (lodInfos.Count == 0) return;
            if (lodInfos[lodInfos.Count - 1] == null) return;
            
            DrawHiddenRange(area, maxProtoDistance / maxDistance, maxProtoDistance);
        }

        /// <summary>
        /// Calculating rect for LOD button
        /// </summary>
        public static Rect CalcLODButton(Rect totalRect, float percentage)
        {
            float rectW = totalRect.width - 43;
            float startX = Mathf.Round(rectW * percentage);

            return new Rect(totalRect.x + startX - 5, totalRect.y, 10, totalRect.height);
        }

        /// <summary>
        /// Calculating rect for drwing slot range box
        /// </summary>
        public static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent, bool hidden = false)
        {
            float rectW = totalRect.width;
            float startX = Mathf.Round(rectW * startPercent);

            float endX = Mathf.Round(rectW * endPercent);

            return new Rect(totalRect.x + startX, totalRect.y, endX - startX, totalRect.height - (hidden ? 14 : 0));
        }

        /// <summary>
        /// Drawing LOD slider box
        /// </summary>
        private static void DrawLODRange(LODGUIInfo currentLOD, float previousLODPercentage, float nextUnits, float maxDistance)
        {
            Color tempColor = GUI.backgroundColor;
            string startPercentageString = currentLOD.LODName + "\n";

            float preMeters = 0f;
            if (preMeters == 0f) preMeters = Mathf.Lerp(0f, maxDistance, previousLODPercentage);
            preMeters = (float)Math.Round(preMeters, 1);

            nextUnits = (float)Math.Round(nextUnits, 1);

            startPercentageString += preMeters.ToString();
            GUIContent content = new GUIContent(startPercentageString);

            GUI.backgroundColor = LODColors[currentLOD.LODIndex];
            GUI.backgroundColor *= 0.6f;
            Styles.LODSliderRangeStyle.Draw(currentLOD.RangeRect, GUIContent.none, false, false, false, false);
            Styles.LODSliderTextStyle.Draw(currentLOD.RangeRect, content, false, false, false, false);

            GUI.backgroundColor = tempColor;
        }

        private static void DrawHiddenRange(Rect totalRect, float cullStartPercent, float maxDist)
        {
            Rect rect = GetHiddenBox(totalRect);
            // Draw the range of a lod level on the slider
            Color preColor = GUI.color;
            Color preBGColor = GUI.backgroundColor;

            if (cullStartPercent > 0f)
                GUI.color = s_hiddenLODColor;
            else
                GUI.color = s_culledLODColor;

            Styles.LODSliderRangeStyle.Draw(GetLookingAwayBox(totalRect, cullStartPercent), GUIContent.none, false, false, false, false);

            GUI.color = preColor;
            GUI.backgroundColor = preBGColor;

            // Draw some details for the current marker
            string startPercentageString = "Looking Away";
            GUIStyle style = new GUIStyle(Styles.LODSliderTextStyle);
            style.alignment = TextAnchor.UpperLeft;
            style.padding = new RectOffset(4, 0, 1, 0);
            style.fontSize = 9;
            style.Draw(GetLookingAwayBox(totalRect, cullStartPercent), new GUIContent(startPercentageString), false, false, false, false);

            if (cullStartPercent > 0f && cullStartPercent != 1)
            {
                GUI.backgroundColor = s_culledLODColor * new Color(1.5f, 1.5f, 1.5f, 0.84f);
                GUI.color = s_culledLODColor * new Color(1.5f, 1.5f, 1.5f, 0.84f);
                Styles.LODSliderRangeStyle.Draw(GetHiddenBox(totalRect, cullStartPercent), GUIContent.none, false, false, false, false);

                GUIStyle culledStyle = new GUIStyle(Styles.LODSliderTextStyle);
                culledStyle.alignment = TextAnchor.UpperLeft;
                culledStyle.padding = new RectOffset(4, 0, 1, 0);
                culledStyle.fontSize = 9;

                GUI.color = preColor;
                GUI.backgroundColor = preBGColor;

                string startCulledString = "Culled > " + Mathf.Round(maxDist);
                culledStyle.Draw(GetHiddenBox(totalRect, cullStartPercent), new GUIContent(startCulledString), false, false, false, false);
            }
        }

        public static Rect GetHiddenBox(Rect totalRect)
        {
            Rect rect = new Rect(totalRect.x, totalRect.y + totalRect.height - 14, totalRect.width, 14);
            return rect;
        }

        public static Rect GetHiddenBox(Rect totalRect, float cullFrom)
        {
            float rectW = totalRect.width;
            float startX = Mathf.Round(rectW * cullFrom);
            float endX = Mathf.Round(rectW);
            
            return new Rect(totalRect.x + startX, totalRect.y + totalRect.height - 14, endX - startX, 14);
        }

        public static Rect GetLookingAwayBox(Rect totalRect, float cullFrom)
        {
            float rectW = totalRect.width;
            float startX = Mathf.Round(rectW * cullFrom);;

            return new Rect(totalRect.x, totalRect.y + totalRect.height - 14, startX, 14);
        }

        public static float[] GetDistanceMeasures(List<LOD> LODs, float bias)
        {
            float[] lods = new float[LODs.Count];
            for (int i = 0; i < LODs.Count; i++) 
            {
                lods[i] = LODs[i].Distance * bias;
            }
            
            return lods;
        }

        public static string GetLODName(int i, int count)
        {
            string name = "LOD " + (i);
            if (i == count) name = "Farthest";
            if (count <= 1) name = "Active";
            return name;
        }
    }
}