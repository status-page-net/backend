using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Api
{
	/// <summary>
	/// Service Business Logic Layer interface.
	/// </summary>
	public interface IServiceBLL
	{
		/// <summary>
		/// Creates the service.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>Instance of the service.</returns>
		/// <exception cref="InvalidServiceException"></exception>
		/// <exception cref="ServiceAlreadyExistsException"></exception>
		Task<Service> CreateAsync(Service service, CancellationToken ct);

		/// <summary>
		/// Gets a service by identifier.
		/// </summary>
		/// <param name="id">Identifier of a service.</param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>Instance of the service or <see langword="null"/> if not exists.</returns>
		Task<Service> GetAsync(Guid id, CancellationToken ct);

		/// <summary>
		/// Updates a service.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="ct">Cancellation token for asynchronous operation.</param>
		/// <returns>
		/// Instance of the service if it was updated or <see langword="null"/> if not exists.
		/// </returns>
		/// <exception cref="InvalidServiceException"></exception>
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

	/// <summary>
	/// Service Data Access Layer interface.
	/// </summary>
	public interface IServiceDAL
	{
		Task<Service> CreateAsync(Service service, CancellationToken ct);
		Task<Service> GetAsync(Guid id, CancellationToken ct);
		Task<Service> UpdateAsync(Service service, CancellationToken ct);
		Task<bool> DeleteAsync(Guid id, CancellationToken ct);
	}
}