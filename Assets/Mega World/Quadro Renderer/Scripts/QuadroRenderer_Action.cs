using UnityEngine;

namespace QuadroRendererSystem
{
    public partial class QuadroRenderer
    {
        private string _extensions = "Extensions";

        public BillboardGenerator CreateBillboardGenerator()
        {
    		GameObject go = GameObject.Find("Quadro Renderer/" + _extensions); 

    		if(go == null)
    		{
    			go = new GameObject("Extensions");
        		go.AddComponent<BillboardGenerator>();
            	go.transform.SetParent(this.transform);
				go.GetComponent<BillboardGenerator>().DetectNecessaryData();
    		}
    		else
    		{
        		go.AddComponent<BillboardGenerator>();
    		}

            return go.GetComponent<BillboardGenerator>();
        }

    	public ColliderSystem CreateColliderSystem()
        {
    		GameObject go = GameObject.Find("Quadro Renderer/" + _extensions); 

    		if(go == null)
    		{
    			go = new GameObject(_extensions);
        		go.AddComponent<ColliderSystem>();
            	go.transform.SetParent(this.transform);
				go.GetComponent<ColliderSystem>().DetectNecessaryData();
    		}
    		else
    		{
        		go.AddComponent<ColliderSystem>();
    		}

            return go.GetComponent<ColliderSystem>();
        }

		public SnapToObject CreateSnapToObject()
        {
    		GameObject go = GameObject.Find("Quadro Renderer/" + _extensions); 

    		if(go == null)
    		{
    			go = new GameObject(_extensions);
        		go.AddComponent<SnapToObject>();
            	go.transform.SetParent(this.transform);
				go.GetComponent<SnapToObject>().DetectNecessaryData();
    		}
    		else
    		{
        		go.AddComponent<SnapToObject>();
    		}

            return go.GetComponent<SnapToObject>();
        }
    }
}