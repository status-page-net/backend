using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace StatusPage.Api.UnitTest
{
	[TestClass]
	public class ServiceFilterTest
	{
		[TestMethod]
		public void Validate_Ok()
		{
			var filter = new ServiceFilter
			{
				Ids = new Guid[] { Guid.NewGuid() },
			};
			filter.Validate();
		}

		[TestMethod]
		public void Validate_Fail_Null()
		{
			ServiceFilter filter = null;
			Assert.ThrowsException<ApiArgumentException>(() => filter.Validate());
		}
	}
}