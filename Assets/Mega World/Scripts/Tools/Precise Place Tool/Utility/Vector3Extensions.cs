#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace MegaWorld
{
    public static class Vector3Extensions
    {
        #region Extension Methods
        public static bool HasZeroComponent(this Vector3 vector, float epsilon = 0.0f)
        {
            float absComp = Mathf.Abs(vector.x);
            if (absComp < epsilon) return true;

            absComp = Mathf.Abs(vector.y);
            if (absComp < epsilon) return true;

            absComp = Mathf.Abs(vector.z);
            if (absComp < epsilon) return true;

            return false;
        }

        public static Vector3 GetSignVector(this Vector3 vector)
        {
            return new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
        }

        public static void ReplaceCoordsValueWith(this Vector3 vector, float valueToReplace, float value)
        {
            if (vector.x == valueToReplace) vector.x = value;
            if (vector.y == valueToReplace) vector.y = value;
            if (vector.z == valueToReplace) vector.z = value;
        }

        public static float AngleWith(this Vector3 thisVector, Vector3 other)
        {
            thisVector.Normalize();
            other.Normalize();

            return Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(thisVector, other), -1.0f, 1.0f));
        }

        public static Vector3 GetVectorWithPositiveComponents(this Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);

            return vector;
        }

        public static Vector3 GetInverse(this Vector3 vector)
        {
            return new Vector3(1.0f / vector.x, 1.0f / vector.y, 1.0f / vector.z);
        }

        public static float GetComponentWithBiggestAbsValue(this Vector3 vector)
        {
            float maxComponent = vector.x;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(vector.y)) maxComponent = vector.y;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(vector.z)) maxComponent = vector.z;

            return maxComponent;
        }

        public static bool IsAlignedWith(this Vector3 vector, Vector3 otherVector)
        {
            vector.Normalize();
            otherVector.Normalize();

            float absDot = Mathf.Abs(Vector3.Dot(vector, otherVector));
            return Mathf.Abs(absDot - 1.0f) < 1e-5f;
        }

        public static bool IsAlignedWith(this Vector3 vector, Vector3 otherVector, out bool pointsInSameDirection)
        {
            pointsInSameDirection = false;

            vector.Normalize();
            otherVector.Normalize();

            float dotProduct = Vector3.Dot(vector, otherVector);
            float absDot = Mathf.Abs(dotProduct);

            if(Mathf.Abs(absDot - 1.0f) < 1e-5f)
            {
                pointsInSameDirection = dotProduct > 0.0f;
                return true;
            }

            return false;
        }

        public static float GetAbsDot(this Vector3 vector, Vector3 otherVector)
        {
            return Mathf.Abs(Vector3.Dot(vector, otherVector));
        }

        public static Vector3 CalculateProjectionPointOnSegment(this Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 segmentDirection = segmentEnd - segmentStart;
            segmentDirection.Normalize();

            Vector3 fromStartPointToPoint = point - segmentStart;
            float dotProduct = Vector3.Dot(segmentDirection, fromStartPointToPoint);
            return segmentStart + segmentDirection * dotProduct;
        }
        #endregion
    }
}
#endif