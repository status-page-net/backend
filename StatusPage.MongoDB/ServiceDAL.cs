using MongoDB.Driver;
using StatusPage.Api;
using System;
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
			Service.Validate(service);

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

		public async Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			using IAsyncCursor<Service> cursor = await Collection.FindAsync(
				service => service.Id == id,
				new FindOptions<Service, Service>
				{
					Limit = 1
				},
				ct);
			bool exists = await cursor.MoveNextAsync(ct);
			if (!exists)
			{
				return null;
			}
			foreach (Service service in cursor.Current)
			{
				return service;
			}
			return null;
		}

		public async Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

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
				Service current = await GetAsync(service.Id, ct);
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