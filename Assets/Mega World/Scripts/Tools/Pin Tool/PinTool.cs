using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VladislavTsurikov;

namespace MegaWorld
{
#if UNITY_EDITOR
    public class PinTool : ToolComponent
    {
        private static int s_pinToolHash = "PinTool".GetHashCode();
        private PinToolEditor _pinToolEditor = new PinToolEditor();
        
        private PlacedObjectInfo _placedObjectInfo = null;

        private float _scaleFactor;
        private float _angle;

        private Vector3 _point;
        private Vector3 _right;
        private Vector3 _upwards;
        private Vector3 _forward;

        private RaycastInfo m_currentRaycast;

        public override void OnGUI()
        {            
            _pinToolEditor.OnGUI(this);
        }

        public override string GetDisplayName() 
        {
            return "Pin";
        }

        public override MegaWorldTools GetTool()
        {
            return MegaWorldTools.Pin;
        }

        public override void DoTool()
        {                        
            if(!IsToolSupportSelectedData())
            {
                return;
            }

            PinToolSettings pinToolSettings  = MegaWorldPath.GeneralDataPackage.PinToolSettings;

            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(s_pinToolHash, FocusType.Passive);

            switch (e.GetTypeForControl(controlID))
            {
            case EventType.MouseDown:
                if (e.button == 0 && !e.alt)
                {
                    Raycast(ref m_currentRaycast);

                    if (m_currentRaycast.isHit)
                    {
                        _placedObjectInfo = PlaceObject(m_currentRaycast);
                        if (_placedObjectInfo == null)
                        {
                            return;
                        }

                        _point = m_currentRaycast.point;
                        GameObjectUtility.GetOrientation(m_currentRaycast.normal, pinToolSettings.FromDirection, pinToolSettings.WeightToNormal, out _upwards, out _right, out _forward);

                        if (pinToolSettings.RotationTransformMode == TransformMode.Fixed)
                        {
                            _placedObjectInfo.gameObject.transform.rotation = 
                                GetRotation(_placedObjectInfo, pinToolSettings.FixedRotationValue);
                        }
                        else
                        {
                            _placedObjectInfo.gameObject.transform.rotation = 
                                GetRotation(_placedObjectInfo, new Vector3(0, _angle, 0));
                        }

                        _scaleFactor = GetObjectScaleFactor(_placedObjectInfo);

                        _placedObjectInfo.gameObject.transform.localScale = new Vector3(0, 0f, 0f);

                        _placedObjectInfo.gameObject.transform.position += new Vector3(0, pinToolSettings.Offset, 0);
                    }

                    GUIUtility.hotControl = controlID;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
            if (GUIUtility.hotControl == controlID && e.button == 0 && _placedObjectInfo != null)
                {
                    if (IntersectsHitPlane(HandleUtility.GUIPointToWorldRay(e.mousePosition), out _point))
                    {
                        Vector3 vector = _point - _placedObjectInfo.RaycastInfo.point;
                        float vectorLength = vector.magnitude;

                        if (vectorLength < 0.01f)
                        {
                            vector = Vector3.up * 0.01f;
                            vectorLength = 0.01f;
                        }

                        _angle = Vector3.Angle(_forward, vector.normalized);
                        if (Vector3.Dot(vector.normalized, _right) < 0.0f)
                        {
                            _angle = -_angle;
                        }

                        float scale = 2.0f * vectorLength * _scaleFactor;
                            
                        switch (pinToolSettings.RotationTransformMode)
				        {
				        	case TransformMode.Free:
				        	{
                                _placedObjectInfo.gameObject.transform.rotation = 
                                    GetRotation(_placedObjectInfo, new Vector3(0, _angle, 0));

				        		break;
				        	}
				        	case TransformMode.Snap:
				        	{				
                                if (pinToolSettings.SnapRotationValue > 0)
                                {
                                    _angle = Mathf.Round(_angle / pinToolSettings.SnapRotationValue) * pinToolSettings.SnapRotationValue;
                                }

                                _placedObjectInfo.gameObject.transform.rotation = 
                                    GetRotation(_placedObjectInfo, new Vector3(0, _angle, 0));

				        		break;
				        	}
				        }

                        switch (pinToolSettings.ScaleTransformMode)
				        {
				        	case TransformMode.Free:
				        	{
                                _placedObjectInfo.gameObject.transform.localScale = new Vector3(scale, scale, scale);

				        		break;
				        	}
				        	case TransformMode.Snap:
				        	{				
                                if (pinToolSettings.SnapScaleValue > 0)
                                {
                                    scale = Mathf.Round(scale / pinToolSettings.SnapScaleValue) * pinToolSettings.SnapScaleValue;
                                    scale = Mathf.Max(scale, 0.01f);
                                }

                                _placedObjectInfo.gameObject.transform.localScale = new Vector3(scale, scale, scale);

				        		break;
				        	}
                            case TransformMode.Fixed:
                            {
                                _placedObjectInfo.gameObject.transform.localScale = pinToolSettings.FixedScaleValue;

                                break;
                            }
				        }
                    }

                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && e.button == 0)
                {
                    if (_placedObjectInfo != null)
                    {
                        if (pinToolSettings.ScaleTransformMode != TransformMode.Fixed)
                        {
                            Vector2 placeSrceenPoint = HandleUtility.WorldToGUIPoint(_placedObjectInfo.RaycastInfo.point);

                            if ((e.mousePosition - placeSrceenPoint).magnitude < 5f)
                            {
                                GameObject.DestroyImmediate(_placedObjectInfo.gameObject);
                            }
                        }

                        _placedObjectInfo = null;
                    }

                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.MouseMove:
                {
                    Raycast(ref m_currentRaycast);

                    e.Use();
                }
                break;
            case EventType.Repaint:
                if (m_currentRaycast.isHit)
                {
                    DrawPinToolHandles();
                }
                break;
            case EventType.Layout:
                HandleUtility.AddDefaultControl(controlID);
                break;
            case EventType.KeyDown:
                switch (e.keyCode)
                {
                case KeyCode.F:
                    // F key - Frame camera on brush hit point
                    if (MegaWorldGUIUtility.IsModifierDown(EventModifiers.None) && m_currentRaycast.isHit)
                    {
                        SceneView.lastActiveSceneView.LookAt(m_currentRaycast.point, SceneView.lastActiveSceneView.rotation, 15);
                        e.Use();
                    }
                    break;
                }
                break;
            }
        }
        
        private void DrawPinToolHandles()
        {
            PinToolSettings pinToolSettings  = MegaWorldPath.GeneralDataPackage.PinToolSettings;

            Vector3 upwards;
            Vector3 right;
            Vector3 forward;

            GameObjectUtility.GetOrientation(m_currentRaycast.normal, pinToolSettings.FromDirection, pinToolSettings.WeightToNormal,
                out upwards, out right, out forward);

            if (_placedObjectInfo == null)
            {
                DrawHandles.DrawXYZCross(m_currentRaycast, upwards, right, forward);

                return;
            }

            Handles.DrawDottedLine(_placedObjectInfo.RaycastInfo.point, _point, 4.0f);

            DrawHandles.DrawXYZCross(m_currentRaycast, upwards, right, forward);
        }

        private Quaternion GetRotation(PlacedObjectInfo placedObjectInfo, Vector3 euler)
        {
            PinToolSettings pinToolSettings = MegaWorldPath.GeneralDataPackage.PinToolSettings;

            Vector3 normal = placedObjectInfo.RaycastInfo.normal;

            Quaternion placeOrientation = Quaternion.LookRotation(_forward, _upwards);
            return placeOrientation * Quaternion.Euler(euler);
        }

        private float GetObjectScaleFactor(PlacedObjectInfo gameObject)
    	{
    		Bounds bounds = gameObject.bounds;
    		Vector3 localScale = gameObject.gameObject.transform.localScale;

    		float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

    		if (size != 0.0f)
    			size = 1.0f / size;
    		else
    			size = 1.0f;

    		return new Vector3(localScale.x * size, localScale.y * size, localScale.z * size).x;
    	}

        private PlacedObjectInfo PlaceObject(RaycastInfo originalRaycastInfo)
        {
            PrototypeGameObject proto = GetRandomSelectedPrototype();

            if(proto == null)
            {
                return null;
            }

            InstanceData spawnInfo = new InstanceData(originalRaycastInfo.hitInfo.point, Vector3.one, Quaternion.identity, 1);

            PlacedObjectInfo objectInfo = GameObjectUtility.PlaceObject(proto, spawnInfo.position, spawnInfo.scale, spawnInfo.rotation);
            MegaWorldPath.GeneralDataPackage.StorageCells.AddItemInstance(proto.ID, objectInfo.gameObject);

            objectInfo.RaycastInfo = originalRaycastInfo;
            GameObjectUtility.ParentGameObject(AllAvailableTypes.GetType(proto), proto, objectInfo);

            return objectInfo;
        }

        private PrototypeGameObject GetRandomSelectedPrototype()
        {
            List<PrototypeGameObject> protoList = MegaWorldPath.DataPackage.BasicData.SelectedVariables.SelectedProtoGameObjectList;

            return protoList[UnityEngine.Random.Range(0, protoList.Count - 1)];
        }

        private bool IntersectsHitPlane(Ray ray, out Vector3 hitPoint)
        {
            float rayDistance;
            Plane plane = new Plane(_upwards, _point);
            if (plane.Raycast(ray, out rayDistance))
            {
                hitPoint = ray.GetPoint(rayDistance);
                return true;
            }
            hitPoint = Vector3.zero;
            return false;
        }

#if UNITY_EDITOR
        public static bool Raycast(ref RaycastInfo raycastInfo)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            foreach (Type type in MegaWorldPath.DataPackage.SelectedVariables.SelectedTypeList)
            {
                Utility.Raycast(ray, out raycastInfo, type.GetCurrentPaintLayers());
            }

            if(raycastInfo.isHit == false)
            {
                return false;
            }

            if(Quaternion.LookRotation(raycastInfo.hitInfo.normal) == new Quaternion(0, 0, 0, 1))
            {
                return false;
            }

            return true;
        }
#endif

        public bool IsToolSupportSelectedData()
        {
            if(MegaWorldPath.DataPackage.SelectedVariables.HasOneSelectedType())
			{
				if(MegaWorldPath.DataPackage.SelectedVariables.SelectedProtoGameObjectList.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
			}
			else
			{
				return false;
			}
        }
    }
#endif
}