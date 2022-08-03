using UnityEngine;

namespace MegaWorld.PrecisePlace
{
    public static class PositionOffset
    {
        public static void ChangePositionOffset(GameObject gameObject, float offset)
        {
            gameObject.transform.position += new Vector3(0, offset, 0);
        }
    }
}