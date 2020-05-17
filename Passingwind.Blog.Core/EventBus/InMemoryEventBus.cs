using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.EventBus
{
	public class InMemoryEventBus : IEventBus
	{
		private static Dictionary<string, IList<Type>> _hubs = new Dictionary<string, IList<Type>>();
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<InMemoryEventBus> _logger;

		public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public void Publish<EventBusData>(EventBusData data) where EventBusData : IEventBusData
		{
			Task.Run(() =>
			{
				var key = data.GetType().FullName;
				_logger.LogDebug("Publish Data '{0}'", key);

				if (_hubs.ContainsKey(key))
				{
					var handles = _hubs[key];

					foreach (var handerType in handles)
					{
						var hander = _serviceProvider.CreateScope().ServiceProvider.GetService(handerType) as IEventBusHandler<EventBusData>;
						if (hander == null)
							throw new Exception($"The handler '{handerType}' not found. do you lost register to services ?");

						try
						{
							AsyncHelper.RunSync(() => hander.HandleAsync(data));
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Execute the handle of '{0}' faild.", handerType.FullName);
						}
					}
				}
			});
		}

		public void Subscribe<EventBusData, EventBusHandler>()
			where EventBusData : IEventBusData
			where EventBusHandler : IEventBusHandler<EventBusData>
		{
			var key = typeof(EventBusData).FullName;
			if (!_hubs.ContainsKey(key))
				_hubs[key] = new List<Type>();

			_hubs[key].Add(typeof(EventBusHandler));
		}

		public void Unsubscribe<EventBusData, EventBusHandler>()
			where EventBusData : IEventBusData
			where EventBusHandler : IEventBusHandler<EventBusData>
		{
			var key = typeof(EventBusData).FullName;
			var handlerType = typeof(EventBusHandler);
			if (_hubs.ContainsKey(key) && _hubs[key].Any(t => t == handlerType))
				_hubs[key].Remove(handlerType);

		}
	}
}
