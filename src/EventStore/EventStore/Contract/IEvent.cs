
namespace EventStore.Contract
{
	public interface IEvent
	{
		string Context { get; }
		string Name { get; }
	}
}
