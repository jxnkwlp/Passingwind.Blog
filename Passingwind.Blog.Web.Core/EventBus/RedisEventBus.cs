using CSRedis;
using Passingwind.Blog.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.EventBus
{
	public class RedisEventBus : IEventBus
	{
		private readonly CSRedisClient _cSRedisClient;

		public RedisEventBus()
		{
			//_cSRedisClient = new CSRedis.CSRedisClient("127.0.0.1:6379,password=123,defaultDatabase=13,prefix=");

		}

		public void Publish<EventBusData>(EventBusData data) where EventBusData : IEventBusData
		{
			throw new NotImplementedException();
		}

		public void Subscribe<EventBusData, EventBusHandler>()
			where EventBusData : IEventBusData
			where EventBusHandler : IEventBusHandler<EventBusData>
		{
			throw new NotImplementedException();
		}

		public void Unsubscribe<EventBusData, EventBusHandler>()
			where EventBusData : IEventBusData
			where EventBusHandler : IEventBusHandler<EventBusData>
		{
			throw new NotImplementedException();
		}
	}
}
