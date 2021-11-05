using StatusPage.Api;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Mock
{
	public class ServiceDALMock : IServiceDAL
	{
		private readonly Dictionary<Guid, Service> _storage = new Dictionary<Guid, Service>();
		private readonly Dictionary<string, Service> _title = new Dictionary<string, Service>();

		public Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			Service clone = service.Clone(refreshETag: true);
			lock (_storage)
			{
				if (_title.ContainsKey(service.Title))
				{
					throw new ServiceAlreadyExistsException(service.Id, null);
				}
				if (!_storage.TryAdd(service.Id, clone))
				{
					throw new ServiceAlreadyExistsException(service.Id, null);
				}
				_title.Add(service.Title, clone);
			}
			return Task.FromResult(clone);
		}

		public Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			Service service;
			lock (_storage)
			{
				_storage.TryGetValue(id, out service);
			}
			return Task.FromResult(service?.Clone(refreshETag: false));
		}

		public Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			lock (_storage)
			{
				bool exists = _storage.TryGetValue(service.Id, out Service current);
				if (!exists)
				{
					return Task.FromResult<Service>(null);
				}
				if (service.ETag != current.ETag)
				{
					throw new OutdatedServiceException(current);
				}

				if (_title.TryGetValue(service.Title, out Service duplicate))
				{
					if (service.Id != duplicate.Id)
					{
						throw new ServiceAlreadyExistsException(service.Id, null);
					}
				}

				Service clone = service.Clone(refreshETag: true);
				_storage[service.Id] = clone;
				_title[service.Title] = clone;
				return Task.FromResult(clone);
			}
		}

		public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			lock (_storage)
			{
				bool exists = _storage.TryGetValue(id, out Service service);
				if (exists)
				{
					_storage.Remove(service.Id);
					_title.Remove(service.Title);
				}
				return Task.FromResult(exists);
			}
		}
	}
}