using UnityEditor.Graphing;

namespace GeoTetra.GTGenericGraph
{
	public interface IGeneratesGraphLogic
	{
		void GenerateLogicChain(LogicChain visitor, SlotReference comingFrom);
	}
}
