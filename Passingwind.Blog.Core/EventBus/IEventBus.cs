using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.EventBus
{
	public interface IEventBus
	{
		void Publish<EventBusData>(EventBusData data) where EventBusData : IEventBusData;

		void Subscribe<EventBusData, EventBusHandler>() where EventBusData : IEventBusData where EventBusHandler : IEventBusHandler<EventBusData>;

		void Unsubscribe<EventBusData, EventBusHandler>() where EventBusData : IEventBusData where EventBusHandler : IEventBusHandler<EventBusData>;
	}
}
