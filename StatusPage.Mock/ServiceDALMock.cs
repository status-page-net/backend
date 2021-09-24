using StatusPage.Api;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Mock
{
	public class ServiceDALMock : IServiceDAL
	{
		private readonly Dictionary<Guid, Service> _Storage = new Dictionary<Guid, Service>();

		public Task CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);
			try
			{
				lock (_Storage)
				{
					_Storage.Add(service.Id, service.Clone());
				}
			}
			catch (ArgumentException e)
			{
				throw new ServiceAlreadyExistsException(service.Id, e);
			}
			return Task.CompletedTask;
		}

		public Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			Service service;
			lock (_Storage)
			{
				_Storage.TryGetValue(id, out service);
			}
			return Task.FromResult(service?.Clone());
		}

		public Task<bool> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			bool exists = false;
			lock (_Storage)
			{
				if (_Storage.ContainsKey(service.Id))
				{
					_Storage[service.Id] = service.Clone();
					exists = true;
				}
			}
			return Task.FromResult(exists);
		}

		public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			bool exists = false;
			lock (_Storage)
			{
				exists = _Storage.Remove(id);
			}
			return Task.FromResult(exists);
		}
	}
}