using System;

namespace StatusPage.Api
{
	[Serializable]
	public class ServicePager
	{
		public const int MinLimit = 1;
		public const int MaxLimit = 1024;

		public int Limit { get; set; } = MaxLimit;

		public const int MinOffset = 0;

		public int Offset { get; set; } = MinOffset;
	}

	public static class ServicePagerExt
	{
		/// <summary>
		/// Validates ServicePager.
		/// </summary>
		/// <exception cref="ApiArgumentException" />
		public static void Validate(this ServicePager pager)
		{
			if (pager == null)
			{
				throw new ApiArgumentException(typeof(ServicePager));
			}
			if (pager.Limit < ServicePager.MinLimit || ServicePager.MaxLimit < pager.Limit)
			{
				throw new ApiArgumentException(typeof(ServicePager), nameof(ServicePager.Limit));
			}
			if (pager.Offset < ServicePager.MinOffset)
			{
				throw new ApiArgumentException(typeof(ServicePager), nameof(ServicePager.Offset));
			}
		}
	}
}