using System.Threading.Tasks;

namespace Passingwind.Blog.EventBus
{
	public interface IEventBusHandler<EventBusData> where EventBusData : IEventBusData
	{
		Task HandleAsync(EventBusData data);
	}
}
