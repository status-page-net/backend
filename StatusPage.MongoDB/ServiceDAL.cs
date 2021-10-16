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

		public async Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			Service clone = service.Clone(refreshETag: true);
			try
			{
				await Collection.InsertOneAsync(clone, null, ct);
			}
			catch (MongoWriteException e) when (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
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
			Service before = await Collection.FindOneAndReplaceAsync<Service>(
				s => (s.Id == service.Id) && (s.ETag == service.ETag),
				clone,
				new FindOneAndReplaceOptions<Service>
				{
					IsUpsert = false,
					ReturnDocument = ReturnDocument.Before
				},
				ct);
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
	}
}