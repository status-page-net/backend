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

		public async Task CreateAsync(Service service, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.PostAsJsonAsync("api/services", service, ct);
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
		}

		public async Task<Service> GetAsync(Guid id, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.GetAsync($"api/services/{id}", ct);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service>(null, ct);
		}

		public async Task<bool> UpdateAsync(Service service, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.PutAsJsonAsync("api/services", service, ct);
			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new InvalidServiceException(message);
			}
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			response.EnsureSuccessStatusCode();
			return true;
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.DeleteAsync($"api/services/{id}", ct);
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
			response.EnsureSuccessStatusCode();
			return true;
		}
	}
}
