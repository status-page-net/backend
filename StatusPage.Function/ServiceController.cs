using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using StatusPage.Api;
using System;
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
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "services")] Service service,
			CancellationToken ct)
		{
			try
			{
				await _serviceBLL.CreateAsync(service, ct);
			}
			catch (InvalidServiceException)
			{
				return new BadRequestResult();
			}
			catch (ServiceAlreadyExistsException)
			{
				return new ConflictResult();
			}
			return new OkResult();
		}

		[FunctionName("Service-Get")]
		public async Task<IActionResult> GetAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "services/{id:Guid}")] HttpRequest request,
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
			[HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "services")] Service service,
			CancellationToken ct)
		{
			try
			{
				bool exists = await _serviceBLL.UpdateAsync(service, ct);
				if (!exists)
				{
					return new NotFoundResult();
				}
			}
			catch (InvalidServiceException)
			{
				return new BadRequestResult();
			}
			return new OkResult();
		}

		[FunctionName("Service-Delete")]
		public async Task<IActionResult> DeleteAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "services/{id:Guid}")] HttpRequest request,
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
	}
}