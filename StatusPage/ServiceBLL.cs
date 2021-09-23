using StatusPage.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.BLL
{
	public class ServiceBLL : IServiceBLL
	{
		private readonly IServiceDAL _serviceDAL;

		public ServiceBLL(IServiceDAL serviceDAL)
		{
			_serviceDAL = serviceDAL ?? throw new ArgumentNullException(nameof(serviceDAL));
		}

		public async Task CreateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			await _serviceDAL.CreateAsync(service, ct);
		}

		public async Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			return await _serviceDAL.GetAsync(id, ct);
		}

		public async Task<bool> UpdateAsync(Service service, CancellationToken ct)
		{
			Service.Validate(service);

			return await _serviceDAL.UpdateAsync(service, ct);
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			return await _serviceDAL.DeleteAsync(id, ct);
		}
	}
}
