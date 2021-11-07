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

		public async Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			service.Validate();

			return await _serviceDAL.CreateAsync(service, ct);
		}

		public async Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct)
		{
			filter.Validate();
			pager.Validate();

			return await _serviceDAL.ListAsync(filter, pager, ct);
		}

		public async Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			service.Validate();

			return await _serviceDAL.UpdateAsync(service, ct);
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			return await _serviceDAL.DeleteAsync(id, ct);
		}
	}
}
