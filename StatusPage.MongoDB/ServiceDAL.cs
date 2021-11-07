using MongoDB.Driver;
using StatusPage.Api;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.MongoDB
{
	public class ServiceDAL : IServiceDAL
	{
		private readonly IMongoDatabase _database;

		public ServiceDAL(IMongoDatabase database)
		{
			_database = database ?? throw new ArgumentNullException(nameof(database));
		}

		private IMongoCollection<Service> Collection => _database.GetCollection<Service>(nameof(Service));

		private const int DuplicateKey = 11000;

		public async Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			service.Validate();

			Service clone = service.Clone(refreshETag: true);
			try
			{
				await Collection.InsertOneAsync(clone, null, ct);
			}
			catch (MongoWriteException e) when (e.WriteError.Code == DuplicateKey)
			{
				throw new ServiceAlreadyExistsException(service.Id, e);
			}
			return clone;
		}

		public async Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct)
		{
			using IAsyncCursor<Service> cursor = await Collection.FindAsync(
				ToFilterDefinition(filter),
				ToFindOptions(pager),
				ct);
			var list = new List<Service>();
			while (await cursor.MoveNextAsync(ct))
			{
				foreach (Service service in cursor.Current)
				{
					list.Add(service);
				}
			}
			return list.ToArray();
		}

		public async Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			service.Validate();

			Service clone = service.Clone(refreshETag: true);
			Service before;
			try
			{
				before = await Collection.FindOneAndReplaceAsync<Service>(
					s => (s.Id == service.Id) && (s.ETag == service.ETag),
					clone,
					new FindOneAndReplaceOptions<Service>
					{
						IsUpsert = false,
						ReturnDocument = ReturnDocument.Before
					},
					ct);
			}
			catch (MongoCommandException e) when (e.Code == DuplicateKey)
			{
				throw new ServiceAlreadyExistsException(service.Id, e);
			}
			if (before == null)
			{
				Service current = await this.GetAsync(service.Id, ct);
				if (current == null)
				{
					return null;
				}
				throw new OutdatedServiceException(current);
			}
			return clone;
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			Service before = await Collection.FindOneAndDeleteAsync(
				service => service.Id == id,
				null,
				ct);
			return (before != null);
		}

		private static FilterDefinition<Service> ToFilterDefinition(ServiceFilter filter)
		{
			FilterDefinitionBuilder<Service> builder = Builders<Service>.Filter;
			var list = new List<FilterDefinition<Service>>();
			if (filter.Ids != null)
			{
				list.Add(builder.In(nameof(Service.Id), filter.Ids));
			}
			if (list.Count == 0)
			{
				return builder.Empty;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return builder.And(list);
		}

		private static FindOptions<Service, Service> ToFindOptions(ServicePager pager)
		{
			return new FindOptions<Service, Service>
			{
				Limit = pager.Limit,
				Skip = pager.Offset
			};
		}

		public async Task UpgradeV1Async(CancellationToken ct)
		{
			await Collection.Indexes.CreateOneAsync(
				new CreateIndexModel<Service>(
					Builders<Service>.IndexKeys.Ascending(nameof(Service.Title)),
					new CreateIndexOptions
					{
						Unique = true,
					}),
				null,
				ct);
		}
	}
}