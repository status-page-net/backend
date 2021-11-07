using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StatusPage.Api.UnitTest
{
	[TestClass]
	public class ServicePagerTest
	{
		[TestMethod]
		public void Validate_Ok()
		{
			var pager = new ServicePager
			{
				Limit = ServicePager.MaxLimit,
				Offset = 10,
			};
			pager.Validate();
		}

		[TestMethod]
		public void Validate_Fail_Null()
		{
			ServicePager pager = null;
			Assert.ThrowsException<ApiArgumentException>(() => pager.Validate());
		}

		[TestMethod]
		public void Validate_Fail_Limit()
		{
			var pager1 = new ServicePager
			{
				Limit = ServicePager.MinLimit - 1,
			};
			Assert.ThrowsException<ApiArgumentException>(() => pager1.Validate());

			var pager2 = new ServicePager
			{
				Limit = ServicePager.MaxLimit + 1,
			};
			Assert.ThrowsException<ApiArgumentException>(() => pager2.Validate());
		}

		[TestMethod]
		public void Validate_Fail_Offset()
		{
			var pager = new ServicePager
			{
				Offset = ServicePager.MinOffset - 1,
			};
			Assert.ThrowsException<ApiArgumentException>(() => pager.Validate());
		}
	}
}