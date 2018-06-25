namespace GeoTetra.GTGenericGraph
{
	public interface IGenericSlotHasValue<T>
	{
		T defaultValue { get; }
		T value { get; }
	}
}
