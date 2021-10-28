using StatusPage.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Client
{
	public class ServiceClient : IServiceBLL
	{
		private readonly HttpClient _client;

		public ServiceClient(HttpClient client)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}

		public async Task<Service> CreateAsync(Service service, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.PostAsJsonAsync("service", service, ct);
			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new InvalidServiceException(message);
			}
			if (response.StatusCode == HttpStatusCode.Conflict)
			{
				throw new ServiceAlreadyExistsException(service.Id, null);
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service>(null, ct);
		}

		public async Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.GetAsync($"service?id={id}", ct);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service>(null, ct);
		}

		public async Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.PutAsJsonAsync("service", service, ct);
			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new InvalidServiceException(message);
			}
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
			if (response.StatusCode == HttpStatusCode.PreconditionFailed)
			{
				Service latest = await response.Content.ReadFromJsonAsync<Service>(null, ct);
				throw new OutdatedServiceException(latest);
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service>(null, ct);
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.DeleteAsync($"service?id={id}", ct);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			response.EnsureSuccessStatusCode();
			return true;
		}
	}
}