using UnityEngine;
using VladislavTsurikov;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuadroRendererSystem
{
    public class Area : MonoBehaviour
    {        
        public Bounds AreaBounds;
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;
        
        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted = false;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected;
        public bool HandlesSettingsFoldout;        

        public void SetAreaBounds()
        {
            AreaBounds = new Bounds();
            AreaBounds.size = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            AreaBounds.center = this.transform.position;
        }

        public Bounds GetAreaBounds()
        {
            AreaBounds = new Bounds();
            AreaBounds.size = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            AreaBounds.center = this.transform.position;
            return AreaBounds;
        }

        public void FitToTerrainSize()
        {
            if(Terrain.activeTerrains.Length == 0)
            {
                Debug.LogError("Could not fit to terrain size - no terrain present");
                return;
            }

            Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
            for (int i = 0; i < Terrain.activeTerrains.Length; i++)
            {
                Terrain terrain = Terrain.activeTerrains[i];

                Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);;
                
                if (i == 0)
                {
                    newBounds = terrainBounds;
                }
                else
                {
                    newBounds.Encapsulate(terrainBounds);
                }
            }

            transform.position = newBounds.center;
            transform.localScale = newBounds.size;
        }

        public void SetBoundsIfNecessary(bool setAllParameters = false)
        {
            bool hasChangedPosition = PastThisPosition != this.transform.position;
            bool hasChangedSize = transform.localScale != PastScale;

            if(setAllParameters == false)
            {
                if(!hasChangedPosition && !hasChangedSize)
                {
                    return;
                }
            }

            if(hasChangedSize || hasChangedPosition || setAllParameters)
            {
                SetAreaBounds();
            }

            PastScale = transform.localScale;

            PastThisPosition = this.transform.position;
        }
    }

#if UNITY_EDITOR
    public class AreaGizmoDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
        static void DrawGizmoForArea(Area area, GizmoType gizmoType)
        {
            bool isFaded = (int)gizmoType == (int)GizmoType.NonSelected || (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType == (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;
            
            if(area.DrawHandleIfNotSelected == false)
            {
                if(isFaded == true)
                {
                    return;
                }
            }

            float opacity = isFaded ? 0.5f : 1.0f;

            DrawBox(area, opacity);
        }

        public static void DrawBox(Area area, float alpha)
        {
            Transform newTransform = area.transform;
            newTransform.rotation = Quaternion.identity;
            newTransform.transform.localScale = new Vector3 (Mathf.Max(1f, newTransform.transform.localScale.z), Mathf.Max(1f, newTransform.transform.localScale.y), Mathf.Max(1f, newTransform.transform.localScale.z));

            if(area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = area.ColorCube;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, area.PixelWidth, area.Dotted);
            }
            else
            {
                float thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }
    }
#endif
}
