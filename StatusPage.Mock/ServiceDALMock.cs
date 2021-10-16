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

		public Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			Service clone = service.Clone(refreshETag: true);
			if (!_storage.TryAdd(service.Id, clone))
			{
				throw new ServiceAlreadyExistsException(service.Id, null);
			}
			return Task.FromResult(clone);
		}

		public Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			_storage.TryGetValue(id, out Service service);
			return Task.FromResult(service?.Clone(refreshETag: false));
		}

		public Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			bool exists = _storage.TryGetValue(service.Id, out Service current);
			if (!exists)
			{
				return Task.FromResult<Service>(null);
			}
			if (service.ETag != current.ETag)
			{
				throw new OutdatedServiceException(current);
			}
			Service clone = service.Clone(refreshETag: true);
			exists = _storage.TryUpdate(service.Id, clone, current);
			if (!exists)
			{
				throw new OutdatedServiceException(current);
			}
			return Task.FromResult(clone);
		}

		public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			bool exists = _storage.TryRemove(id, out _);
			return Task.FromResult(exists);
		}
	}
}