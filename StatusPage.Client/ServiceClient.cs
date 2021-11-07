using StatusPage.Api;
using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
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
				throw new ApiArgumentException(message);
			}
			if (response.StatusCode == HttpStatusCode.Conflict)
			{
				throw new ServiceAlreadyExistsException(service.Id, null);
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service>(null, ct);
		}

		public async Task<Service[]> ListAsync(ServiceFilter filter, ServicePager pager, CancellationToken ct)
		{
			filter.Validate();
			pager.Validate();

			var qsb = new QueryStringBuilder();
			qsb.Append(nameof(filter), filter);
			qsb.Append(nameof(pager), pager);

			using HttpResponseMessage response = await _client.GetAsync(
				$"service{qsb}",
				ct);
			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new ApiArgumentException(message);
			}
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<Service[]>(null, ct);
		}

		public async Task<Service> UpdateAsync(Service service, CancellationToken ct)
		{
			using HttpResponseMessage response = await _client.PutAsJsonAsync("service", service, ct);
			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				string message = await response.Content.ReadAsStringAsync();
				throw new ApiArgumentException(message);
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