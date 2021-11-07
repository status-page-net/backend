using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.MongoDB
{
	public class Schema
	{
		private readonly IMongoDatabase _database;

		public Schema(IMongoDatabase database)
		{
			_database = database ?? throw new ArgumentNullException(nameof(database));
		}

		private IMongoCollection<Version> Collection => _database.GetCollection<Version>(nameof(Version));

		public async Task<Version> CreateAsync(int id, CancellationToken ct)
		{
			var version = new Version
			{
				Id = id
			};
			await Collection.InsertOneAsync(version, null, ct);
			return version;
		}

		public async Task<Version> GetLastAsync(CancellationToken ct)
		{
			using IAsyncCursor<Version> cursor = await Collection.FindAsync(
				Builders<Version>.Filter.Empty,
				new FindOptions<Version, Version>
				{
					Limit = 1,
					Sort = Builders<Version>.Sort.Descending(nameof(Version.Id))
				},
				ct);
			bool exists = await cursor.MoveNextAsync(ct);
			if (!exists)
			{
				return null;
			}
			foreach (Version version in cursor.Current)
			{
				return version;
			}
			return null;
		}

		private async Task UpgradeV1Async(CancellationToken ct)
		{
			var serviceDAL = new ServiceDAL(_database);
			await serviceDAL.UpgradeV1Async(ct);
		}

		public async Task<Version> UpgradeAsync(CancellationToken ct)
		{
			int lastId = 0;
			Version version = await GetLastAsync(ct);
			if (version != null)
			{
				lastId = version.Id;
			}

			var list = new Func<CancellationToken, Task>[]
				{
					null,
					async (CancellationToken ct) => await UpgradeV1Async(ct),
				};
			for (int i = lastId + 1; i < list.Length; i++)
			{
				await list[i](ct);
				version = await CreateAsync(i, ct);
			}

			return version;
		}
	}
}