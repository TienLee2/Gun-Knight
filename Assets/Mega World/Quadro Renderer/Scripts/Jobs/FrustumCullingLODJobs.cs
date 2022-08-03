using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace QuadroRendererSystem
{
    [BurstCompile(CompileSynchronously = true)]
    public struct FrustumCullingLODJob : IJob
    {
        [ReadOnly]
        public NativeList<Matrix4x4> MatrixList;
        [ReadOnly]
        public NativeArray<Plane> FrustumPlanes;
        [ReadOnly]
        public NativeArray<float> LodDistances;

        public NativeList<Matrix4x4> ItemLOD0MatrixList;
        public NativeList<Matrix4x4> ItemLOD1MatrixList;
        public NativeList<Matrix4x4> ItemLOD2MatrixList;
        public NativeList<Matrix4x4> ItemLOD3MatrixList;
        public NativeList<Matrix4x4> ItemLOD4MatrixList;
        public NativeList<Matrix4x4> ItemLOD5MatrixList;
        public NativeList<Matrix4x4> ItemLOD6MatrixList;
        public NativeList<Matrix4x4> ItemLOD7MatrixList;

        public NativeList<Matrix4x4> ItemLOD0ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD1ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD2ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD3ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD4ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD5ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD6ShadowMatrixList;
        public NativeList<Matrix4x4> ItemLOD7ShadowMatrixList;

        public NativeList<Vector4> LOD0FadeList;
        public NativeList<Vector4> LOD1FadeList;
        public NativeList<Vector4> LOD2FadeList;
        public NativeList<Vector4> LOD3FadeList;
        public NativeList<Vector4> LOD4FadeList;
        public NativeList<Vector4> LOD5FadeList;
        public NativeList<Vector4> LOD6FadeList;
        public NativeList<Vector4> LOD7FadeList;
        
        public float ShadowDistance;
        public GetAdditionalShadow GetAdditionalShadow;
        public float MinCullingDistance;
        public float IncreaseBoundingSphereForShadows;

        public bool IsFrustumCulling;

        public float MaxDistance;
        public float3 CameraPosition;
        public float BoundingSphereRadius;

        public float LODFadeDistance;
        public bool LodFadeForLastLOD;

        public int LODCount;

        public bool UseLODFade;

        public Vector3 FloatingOriginOffset;

        public Vector3 DirectionLight;
        public Vector3 BoundsSize;

        public void Execute()
        {       
            for (int i = MatrixList.Length - 1; i >= 0; i--)
            {
                Matrix4x4 matrix = MatrixList[i];
                matrix = TranslateMatrix(matrix, FloatingOriginOffset);

                float3 position = ExtractPositionFromMatrix(matrix);
                float distanceToCamera = math.distance(CameraPosition, position);

                if(DistanceCulling(distanceToCamera))
	            {
	            	continue;
	            }

                int lodIndex = 9;
	            Vector4 lodFade = new Vector4(0, 0, 0, 0);

                if (IsFrustumCulling && FrustumCulling(position, 0))
	            {
                    if(AddAdditionalShadow(distanceToCamera, position))
		            {
                        CalculateLODWithoutLODFade(distanceToCamera, out lodIndex);

			            AddShadow(matrix, distanceToCamera, lodIndex);

                        if(UseLODFade)
                        {
                            AddLODFade(lodIndex, new Vector4(0, 0, 0, 0));
                        }
                    }

                    continue;
                }

                CalculateLOD(distanceToCamera, out lodIndex, out lodFade);

	            if(lodIndex != 9)
	            {
                    if(UseLODFade)
                    {
                        if(lodFade.x != 0)
	            	    {
                            AddShadow(matrix, distanceToCamera, lodIndex + 1);
                            AddVisibleMesh(matrix, lodIndex + 1);
                            AddLODFade(lodIndex + 1, new Vector4(0, 0, 0, 0));
	            	    }

                        AddLODFade(lodIndex, lodFade);
                    }

	            	AddShadow(matrix, distanceToCamera, lodIndex);
	            	AddVisibleMesh(matrix, lodIndex);
	            }
            }
        }

        void CalculateLODFade(float cameraDistance, int nextLOD, out Vector4 lodFade)
        {
        	lodFade = new Vector4(0, 0, 0, 0);
        	float fade = 0;

        	float nextLODDistance = LodDistances[nextLOD];
            float distance = nextLODDistance - cameraDistance;

            if (distance <= LODFadeDistance)
            {
                fade = Mathf.Clamp01(distance / LODFadeDistance);

                fade = Mathf.Clamp(fade, 0.1f, 1f);

        		if(fade != 0)
        		{
        			float lodFadeQuantified = 1 - Mathf.Clamp(Mathf.RoundToInt(fade * 16) / 16f, 0.0625f, 1f);
        			lodFade = new Vector4(fade, lodFadeQuantified, 0, 0);
        		}
            }
        }

        void CalculateLOD(float distanceToCamera, out int lod, out Vector4 lodFade)
        {
            lod = 9;
        	lodFade = new Vector4(0, 0, 0, 0);

            for (int i = 0; i < LODCount; i++)
            {
                if (distanceToCamera <= LodDistances[i])
                {
                    lod = i;

        			if(i != LODCount - 1)
        			{
        				if (UseLODFade)
                		{
        					if(LodFadeForLastLOD)
        					{
        						if(i == LODCount - 2)
        						{
                					CalculateLODFade(distanceToCamera, i, out lodFade);
        						}
        					}
        					else
        					{
        						CalculateLODFade(distanceToCamera, i, out lodFade);
        					}
        				}
        			}

                    break;
                }
            }
        }

        void CalculateLODWithoutLODFade(float distanceToCamera, out int lod)
        {
            lod = 9;

            for (int i = 0; i < LODCount; i++)
            {
        		int nextLOD = i + 1;
                if (distanceToCamera <= LodDistances[nextLOD])
                {
                    lod = i;

                    break;
                }
            }
        }

        void AddLODFade(int lodIndex, Vector4 lodFade)
        {
        	switch (lodIndex)
            {
                case 0: 
                {
                    LOD0FadeList.Add(lodFade);

                    break;
                }
                case 1:
                {
        			LOD1FadeList.Add(lodFade);

                    break;
                }
        		case 2:
                {
        			LOD2FadeList.Add(lodFade);

                    break;
                }
        		case 3:
                {
        			LOD3FadeList.Add(lodFade);

                    break;
                }
                case 4:
                {
        			LOD4FadeList.Add(lodFade);

                    break;
                }
                case 5:
                {
        			LOD5FadeList.Add(lodFade);

                    break;
                }
                case 6:
                {
        			LOD6FadeList.Add(lodFade);

                    break;
                }
                case 7:
                {
        			LOD7FadeList.Add(lodFade);

                    break;
                }
            }
        }

        void AddShadow(Matrix4x4 matrix, float distanceToCamera, int lodIndex)
        {
        	if(distanceToCamera > ShadowDistance)
            {
                return;
            }

        	switch (lodIndex)
            {
                case 0: 
                {
                    ItemLOD0ShadowMatrixList.Add(matrix);

                    break;
                }
                case 1:
                {
        			ItemLOD1ShadowMatrixList.Add(matrix);

                    break;
                }
        		case 2:
                {
        			ItemLOD2ShadowMatrixList.Add(matrix);

                    break;
                }
        		case 3:
                {
        			ItemLOD3ShadowMatrixList.Add(matrix);

                    break;
                }
                case 4:
                {
        			ItemLOD4ShadowMatrixList.Add(matrix);

                    break;
                }
                case 5:
                {
        			ItemLOD5ShadowMatrixList.Add(matrix);

                    break;
                }
                case 6:
                {
        			ItemLOD6ShadowMatrixList.Add(matrix);

                    break;
                }
                case 7:
                {
        			ItemLOD7ShadowMatrixList.Add(matrix);

                    break;
                }
            }
        }

        void AddVisibleMesh(Matrix4x4 matrix, int lodIndex)
        {
        	switch (lodIndex)
            {
                case 0: 
                {
                    ItemLOD0MatrixList.Add(matrix);

                    break;
                }
                case 1:
                {
        			ItemLOD1MatrixList.Add(matrix);

                    break;
                }
        		case 2:
                {
        			ItemLOD2MatrixList.Add(matrix);

                    break;
                }
        		case 3:
                {
        			ItemLOD3MatrixList.Add(matrix);

                    break;
                }
                case 4:
                {
        			ItemLOD4MatrixList.Add(matrix);

                    break;
                }
                case 5:
                {
        			ItemLOD5MatrixList.Add(matrix);

                    break;
                }
                case 6:
                {
        			ItemLOD6MatrixList.Add(matrix);

                    break;
                }
                case 7:
                {
        			ItemLOD7MatrixList.Add(matrix);

                    break;
                }
            }
        }

        bool DistanceCulling(float distanceToCamera)
        {
        	if (distanceToCamera > MaxDistance)
            {
                return true;
            }

        	return false;
        }

        private bool AddAdditionalShadow(float distanceToCamera, Vector3 position)
        {
            if(distanceToCamera > ShadowDistance)
            {
                return false;
            }

            switch (GetAdditionalShadow)
            {
                case GetAdditionalShadow.MinCullingDistance:
                {
                    if(distanceToCamera <= MinCullingDistance)
                    {
                        return true;
                    }
                    break;
                }
                case GetAdditionalShadow.IncreaseBoundingSphere:
                {
                    if (FrustumCulling(position, IncreaseBoundingSphereForShadows) == false)
                    {
                        return true;
                    }

                    break;
                }
                case GetAdditionalShadow.DirectionLightShadowVisible:
                {
                    Bounds bounds = new Bounds(position, BoundsSize);
                    Vector3 planeOrigin = new Vector3(0, position.y - BoundsSize.y, 0);
                    if (IsShadowVisible(bounds, DirectionLight, planeOrigin, FrustumPlanes))
                    {
                        return true;
                    }

                    break;
                }
            }

            return false;
        }

        private bool FrustumCulling(Vector3 position, float increaseBounding = 0)
        {
            BoundingSphere boundingSphere = new BoundingSphere(position, BoundingSphereRadius + increaseBounding);

            for (int i = 0; i <= FrustumPlanes.Length - 1; i++)
            {
                float dist = FrustumPlanes[i].normal.x * boundingSphere.position.x +
                             FrustumPlanes[i].normal.y * boundingSphere.position.y +
                             FrustumPlanes[i].normal.z * boundingSphere.position.z + FrustumPlanes[i].distance;
                if (dist < -boundingSphere.radius)
                {
                    return true;
                }
            }

            return false;
        }

        private float3 ExtractPositionFromMatrix(Matrix4x4 matrix)
        {
            float3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        private Matrix4x4 TranslateMatrix(Matrix4x4 matrix, float3 offset)
        {
            Matrix4x4 translatedMatrix = matrix;
            translatedMatrix.m03 = matrix.m03 + offset.x;
            translatedMatrix.m13 = matrix.m13 + offset.y;
            translatedMatrix.m23 = matrix.m23 + offset.z;
            return translatedMatrix;
        }

        public static bool IsShadowVisible(Bounds objectBounds, Vector3 lightDirection, Vector3 planeOrigin, NativeArray<Plane> frustumPlanes)
        {
            bool hitPlane;
            Bounds shadowBounds = GetShadowBounds(objectBounds, lightDirection, planeOrigin, out hitPlane);

            return hitPlane && BoundsIntersectsFrustum(frustumPlanes, shadowBounds);
        }

        public static Bounds GetShadowBounds(Bounds objectBounds, Vector3 lightDirection, Vector3 planeOrigin, out bool hitPlane)
        {
            Ray p0 = new Ray(new Vector3(objectBounds.min.x, objectBounds.max.y, objectBounds.min.z), lightDirection);
            Ray p1 = new Ray(new Vector3(objectBounds.min.x, objectBounds.max.y, objectBounds.max.z), lightDirection);
            Ray p2 = new Ray(new Vector3(objectBounds.max.x, objectBounds.max.y, objectBounds.min.z), lightDirection);
            Ray p3 = new Ray(objectBounds.max, lightDirection);

            Vector3 hitPoint;
            hitPlane = false;

            if (IntersectPlane(p0, planeOrigin, out hitPoint))
            {
                objectBounds.Encapsulate(hitPoint);
                hitPlane = true;
            }

            if (IntersectPlane(p1, planeOrigin, out hitPoint))
            {
                objectBounds.Encapsulate(hitPoint);
                hitPlane = true;
            }

            if (IntersectPlane(p2, planeOrigin, out hitPoint))
            {
                objectBounds.Encapsulate(hitPoint);
                hitPlane = true;
            }

            if (IntersectPlane(p3, planeOrigin, out hitPoint))
            {
                objectBounds.Encapsulate(hitPoint);
                hitPlane = true;
            }
            return objectBounds;
        }

        public static bool IntersectPlane(Ray ray, Vector3 planeOrigin, out Vector3 hitPoint)
        {
            Vector3 planeNormal = -Vector3.up;
            float denominator = Vector3.Dot(ray.direction, planeNormal);
            if (denominator > 0.00001f)
            {
                float t = Vector3.Dot(planeOrigin - ray.origin, planeNormal) / denominator;
                hitPoint = ray.origin + ray.direction * t;
                return true;
            }

            hitPoint = Vector3.zero;
            return false;
        }

        public static bool BoundsIntersectsFrustum(NativeArray<Plane> planes, Bounds bounds)
        {
            var center = bounds.center;
            var extents = bounds.extents;

            for (int i = 0; i <= planes.Length - 1; i++)
            {
                Vector3 planeNormal = planes[i].normal;
                float planeDistance = planes[i].distance;

                Vector3 abs = new Vector3(Mathf.Abs(planeNormal.x), Mathf.Abs(planeNormal.y), Mathf.Abs(planeNormal.z));
                float r = extents.x * abs.x + extents.y * abs.y + extents.z * abs.z;
                float s = planeNormal.x * center.x + planeNormal.y * center.y + planeNormal.z * center.z;
                float value = s + r;
                if (s + r < -planeDistance)
                {
                    return false;
                }
            }
            return true;
        }
    }
}