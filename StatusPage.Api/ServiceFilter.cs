using System;
using System.Collections.Generic;

namespace StatusPage.Api
{
	[Serializable]
	public class ServiceFilter
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public static class ServiceFilterExt
	{
		/// <summary>
		/// Validates ServiceFilter.
		/// </summary>
		/// <exception cref="ApiArgumentException" />
		public static void Validate(this ServiceFilter filter)
		{
			if (filter == null)
			{
				throw new ApiArgumentException(typeof(ServiceFilter));
			}
		}
	}
}