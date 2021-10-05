using StatusPage.Api;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Mock
{
	public class ServiceDALMock : IServiceDAL
	{
		private readonly ConcurrentDictionary<Guid, Service> _storage = new ConcurrentDictionary<Guid, Service>();

		public Task CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			if (!_storage.TryAdd(service.Id, service.Clone()))
			{
				throw new ServiceAlreadyExistsException(service.Id, null);
			}
			return Task.CompletedTask;
		}

		public Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			_storage.TryGetValue(id, out Service service);
			return Task.FromResult(service?.Clone());
		}

		public Task<bool> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			bool exists = _storage.TryGetValue(service.Id, out Service current);
			if (!exists)
			{
				return Task.FromResult(exists);
			}
			exists = _storage.TryUpdate(service.Id, service, current);
			return Task.FromResult(exists);
		}

		public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			bool exists = _storage.TryRemove(id, out _);
			return Task.FromResult(exists);
		}
	}
}