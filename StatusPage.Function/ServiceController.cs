using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using StatusPage.Api;
using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Function
{
	public class ServiceController
	{
		private readonly IServiceBLL _serviceBLL;

		public ServiceController(IServiceBLL serviceBLL)
		{
			_serviceBLL = serviceBLL ?? throw new ArgumentNullException(nameof(serviceBLL));
		}

		[FunctionName("Service-Create")]
		public async Task<IActionResult> CreateAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "services")] HttpRequest request,
			CancellationToken ct)
		{
			var service = await FromJsonAsync<Service>(request, ct);

			try
			{
				Service created = await _serviceBLL.CreateAsync(service, ct);
				return new OkObjectResult(created);
			}
			catch (InvalidServiceException e)
			{
				return new BadRequestObjectResult(e.Message);
			}
			catch (ServiceAlreadyExistsException e)
			{
				return new ConflictObjectResult(e.Message);
			}
		}

		[FunctionName("Service-Get")]
		public async Task<IActionResult> GetAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "services/{id:Guid}")] HttpRequest request,
			Guid id,
			CancellationToken ct)
		{
			Service service = await _serviceBLL.GetAsync(id, ct);
			if (service == null)
			{
				return new NotFoundResult();
			}
			return new OkObjectResult(service);
		}

		[FunctionName("Service-Update")]
		public async Task<IActionResult> UpdateAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "services")] HttpRequest request,
			CancellationToken ct)
		{
			var service = await FromJsonAsync<Service>(request, ct);

			try
			{
				Service updated = await _serviceBLL.UpdateAsync(service, ct);
				if (updated == null)
				{
					return new NotFoundResult();
				}
				return new OkObjectResult(updated);
			}
			catch (InvalidServiceException e)
			{
				return new BadRequestObjectResult(e.Message);
			}
			catch (OutdatedServiceException e)
			{
				return new ObjectResult(e.Latest)
				{
					StatusCode = (int)HttpStatusCode.PreconditionFailed
				};
			}
		}

		[FunctionName("Service-Delete")]
		public async Task<IActionResult> DeleteAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "services/{id:Guid}")] HttpRequest request,
			Guid id,
			CancellationToken ct)
		{
			bool exists = await _serviceBLL.DeleteAsync(id, ct);
			if (!exists)
			{
				return new NotFoundResult();
			}
			return new OkResult();
		}

		private static async Task<T> FromJsonAsync<T>(HttpRequest request, CancellationToken ct)
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			return await JsonSerializer.DeserializeAsync<T>(request.Body, options, ct);
		}
	}
}