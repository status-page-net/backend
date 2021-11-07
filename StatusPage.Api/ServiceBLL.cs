using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Api
{
	/// <summary>
	/// Service Business Logic Layer (BLL) interface.
	/// </summary>
	public interface IServiceBLL
	{
		/// <summary>
		/// Creates the service.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>Instance of the service.</returns>
		/// <exception cref="ApiArgumentException"></exception>
		/// <exception cref="ServiceAlreadyExistsException"></exception>
		Task<Service> CreateAsync(Service service, CancellationToken ct);

		/// <summary>
		/// List services by filter and pager.
		/// </summary>
		/// <param name="filter">Filter.</param>
		/// <param name="pager">Pager.</param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>Array of services.</returns>
		/// <exception cref="ApiArgumentException"></exception>
		Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct);

		/// <summary>
		/// Updates a service.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>
		/// Instance of the service if it was updated or <see langword="null"/> if not exists.
		/// </returns>
		/// <exception cref="ApiArgumentException"></exception>
		/// <exception cref="OutdatedServiceException"></exception>
		Task<Service> UpdateAsync(Service service, CancellationToken ct);

		/// <summary>
		/// Deletes the service by identifier.
		/// </summary>
		/// <param name="id">Identifier of the service.</param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>
		/// <see langword="true"/> if service was deleted and <see langword="false"/> if not exists.
		/// </returns>
		Task<bool> DeleteAsync(Guid id, CancellationToken ct);
	}

	public static class ServiceBLLExt
	{
		public static async Task<Service> GetAsync(this IServiceBLL serviceBLL, Guid id, CancellationToken ct)
		{
			var filter = new ServiceFilter
			{
				Ids = new Guid[] { id }
			};
			var pager = new ServicePager
			{
				Limit = 1
			};
			Service[] list = await serviceBLL.ListAsync(filter, pager, ct);
			if (list.Length == 0)
			{
				return null;
			}
			return list[0];
		}
	}
}