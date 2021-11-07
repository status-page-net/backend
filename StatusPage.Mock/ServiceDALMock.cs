using StatusPage.Api;
using System;
using System.Collections.Generic;
using System.Linq;
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
			service.Validate();

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

		public Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct)
		{
			filter.Validate();
			pager.Validate();

			var predicates = new List<Func<Service, bool>>();
			if (filter.Ids != null)
			{
				predicates.Add((Service service) =>
				{
					return filter.Ids.Contains(service.Id);
				});
			}

			Service[] array;
			lock (_storage)
			{
				// Full scan. Ineffective but ok for Mock.
				array = _storage
					.Select((KeyValuePair<Guid, Service> pair) =>
					{
						Service service = pair.Value;
						return service.Clone(refreshETag: false);
					})
					.Where((Service service) =>
					{
						foreach (Func<Service, bool> predicate in predicates)
						{
							if (!predicate(service))
							{
								return false;
							}
						}
						return true;
					})
					.Skip(pager.Offset)
					.Take(pager.Limit)
					.ToArray();
			}
			return Task.FromResult(array);
		}

		public Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			service.Validate();

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