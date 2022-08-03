using UnityEngine;

namespace QuadroRendererSystem
{
    [ExecuteInEditMode]
    public class BillboardGenerator : MonoBehaviour
    {
        public QuadroRenderer QuadroRenderer;

        void OnEnable()
        {
            DetectNecessaryData();

            if(HasAllNecessaryData())
            {
                QuadroRenderer.BillboardGenerator = this;
            }
        }

        public void DetectNecessaryData()
        {
            if (QuadroRenderer == null)
            {
                QuadroRenderer = GetComponentInParent<QuadroRenderer>();
            }
        }

        public bool HasAllNecessaryData()
        {
            if(QuadroRenderer == null)
            {
                return false;
            }

            return true;
        }
    }
}