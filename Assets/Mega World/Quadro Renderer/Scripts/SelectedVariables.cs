using System.Collections.Generic;

namespace QuadroRendererSystem
{
    public class SelectedVariables
	{
		public List<int> SelectedProtoIndexList = new List<int>();

		public void RefreshSelectedParameters(QuadroPrototypesPackage quadroPrototypesPackage)
		{
			ClearSelectedList();
            SetSelectedList(quadroPrototypesPackage);
		}

		private void ClearSelectedList()
		{
			SelectedProtoIndexList.Clear();
		}

		private void SetSelectedList(QuadroPrototypesPackage quadroPrototypesPackage)
		{
		    for (int index = 0; index < quadroPrototypesPackage.PrototypeList.Count; index++)
		    {
		    	if(quadroPrototypesPackage.PrototypeList[index].Selected)
		    	{
					SelectedProtoIndexList.Add(index);
		    	}
		    }
		}
	}
}