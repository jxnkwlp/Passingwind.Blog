using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class WidgetDynamicContentService : Service<WidgetDynamicContent>, IWidgetDynamicContentService
	{
		public WidgetDynamicContentService(IRepository<WidgetDynamicContent, int> repository) : base(repository)
		{
		}

		public override async Task<WidgetDynamicContent> GetByIdAsync(int key, CancellationToken cancellationToken = default)
		{
			return await Repository.Includes(t => t.Properties).FirstOrDefaultAsync(t => t.Id == key);
		}

		public override async Task DeleteByIdAsync(int key, CancellationToken cancellationToken = default)
		{
			var entity = await Repository.Includes(t => t.Properties).FirstOrDefaultAsync(t => t.Id == key);

			if (entity != null)
				await Repository.DeleteAsync(entity);
		}

		public async Task DeleteAsync(Guid widgetId, string userId)
		{
			if (!string.IsNullOrWhiteSpace(userId))
				await Repository.DeleteByWhereAsync(t => t.WidgetId == widgetId && t.UserId == userId);
			else
				await Repository.DeleteByWhereAsync(t => t.WidgetId == widgetId);
		}

		public Task DeleteAsync(Guid widgetId, string userId, Dictionary<string, object> properties)
		{
			throw new NotImplementedException();
		}

		public async Task<T> GetAsync<T>(Guid widgetId, string userId) where T : IWidgetDynamicContent
		{
			var query = Repository.Includes(t => t.Properties);

			WidgetDynamicContent entity;

			if (!string.IsNullOrWhiteSpace(userId))
				entity = await query.FirstOrDefaultAsync(t => t.WidgetId == widgetId & t.UserId == userId);
			else
				entity = await query.FirstOrDefaultAsync(t => t.WidgetId == widgetId);

			if (entity == null)
				return default;

			return entity.ToDynamicContent<T>();
		}

		public async Task<IEnumerable<T>> GetListAsync<T>(Guid widgetId, string userId) where T : IWidgetDynamicContent
		{
			IEnumerable<WidgetDynamicContent> list;

			if (!string.IsNullOrWhiteSpace(userId))
				list = await Repository.Includes(t => t.Properties).Where(t => t.WidgetId == widgetId & t.UserId == userId).ToListAsync();
			else
				list = await Repository.Includes(t => t.Properties).Where(t => t.WidgetId == widgetId).ToListAsync();

			return list.Select(t => t.ToDynamicContent<T>()).ToArray();
		}

		public async Task<WidgetDynamicContent> InsertAsync(IWidgetDynamicContent content)
		{
			var entity = content.ToContent();

			await Repository.InsertAsync(entity);

			return entity;
		}

		public async Task<WidgetDynamicContent> UpdateAsync(IWidgetDynamicContent content)
		{
			var dyContent = content.ToContent();

			if (dyContent.Id <= 0)
				throw new Exception("Can't update.");

			var entity = await GetByIdAsync(dyContent.Id);

			entity.WidgetId = dyContent.WidgetId;
			entity.UserId = dyContent.UserId;

			// TODO  update it once

			await Repository.LoadCollectionAsync(entity, t => t.Properties);

			entity.Properties = dyContent.Properties;

			await Repository.UpdateAsync(entity);

			return dyContent;
		}
	}
}
