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
			catch (ApiArgumentException e)
			{
				return BadRequest(e.Message);
			}
			catch (ServiceAlreadyExistsException e)
			{
				return Conflict(e.Message);
			}
		}

		[HttpGet]
		public async Task<ActionResult<Service[]>> ListAsync(
			[FromQuery] ServiceFilter filter,
			[FromQuery] ServicePager pager,
			CancellationToken ct)
		{
			try
			{
				Service[] list = await _serviceBLL.ListAsync(filter, pager, ct);
				return Ok(list);
			}
			catch (ApiArgumentException e)
			{
				return BadRequest(e.Message);
			}
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
			catch (ApiArgumentException e)
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