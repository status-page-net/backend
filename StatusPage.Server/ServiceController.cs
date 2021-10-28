using Microsoft.AspNetCore.Mvc;
using StatusPage.Api;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace StatusPage.Server
{
	[ApiController]
	[Route("Service")]
	public class ServiceController : ControllerBase
	{
		private readonly IServiceBLL _serviceBLL;

		public ServiceController(IServiceBLL serviceBLL)
		{
			_serviceBLL = serviceBLL ?? throw new ArgumentNullException(nameof(serviceBLL));
		}

		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] Service service, CancellationToken ct)
		{
			try
			{
				Service created = await _serviceBLL.CreateAsync(service, ct);
				return Ok(created);
			}
			catch (InvalidServiceException e)
			{
				return BadRequest(e.Message);
			}
			catch (ServiceAlreadyExistsException e)
			{
				return Conflict(e.Message);
			}
		}

		[HttpGet]
		public async Task<ActionResult<Service>> GetAsync([FromQuery] Guid id, CancellationToken ct)
		{
			Service service = await _serviceBLL.GetAsync(id, ct);
			if (service == null)
			{
				return NotFound();
			}
			return Ok(service);
		}

		[HttpPut]
		public async Task<ActionResult> UpdateAsync([FromBody] Service service, CancellationToken ct)
		{
			try
			{
				Service updated = await _serviceBLL.UpdateAsync(service, ct);
				if (updated == null)
				{
					return NotFound();
				}
				return Ok(updated);
			}
			catch (InvalidServiceException e)
			{
				return BadRequest(e.Message);
			}
			catch (OutdatedServiceException e)
			{
				return StatusCode(
					(int)HttpStatusCode.PreconditionFailed,
					e.Latest);
			}
		}

		[HttpDelete]
		public async Task<ActionResult> DeleteAsync([FromQuery] Guid id, CancellationToken ct)
		{
			bool exists = await _serviceBLL.DeleteAsync(id, ct);
			if (!exists)
			{
				return NotFound();
			}
			return Ok();
		}
	}
}