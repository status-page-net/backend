using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Api
{
	/// <summary>
	/// Service Data Access Layer (DAL) interface.
	/// </summary>
	public interface IServiceDAL
	{
		Task<Service> CreateAsync(Service service, CancellationToken ct);
		Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct);
		Task<Service> UpdateAsync(Service service, CancellationToken ct);
		Task<bool> DeleteAsync(Guid id, CancellationToken ct);
	}

	public static class ServiceDALExt
	{
		public static async Task<Service> GetAsync(this IServiceDAL serviceDAL, Guid id, CancellationToken ct)
		{
			var filter = new ServiceFilter
			{
				Ids = new Guid[] { id }
			};
			var pager = new ServicePager
			{
				Limit = 1
			};
			Service[] list = await serviceDAL.ListAsync(filter, pager, ct);
			if (list.Length == 0)
			{
				return null;
			}
			return list[0];
		}
	}
}