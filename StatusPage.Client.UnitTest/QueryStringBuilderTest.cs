using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatusPage.Api;
using System;

namespace StatusPage.Client.UnitTest
{
	[TestClass]
	public class QueryStringBuilderTest
	{
		[TestMethod]
		public void Append_Ok_Empty()
		{
			var builder = new QueryStringBuilder();
			var filter = new ServiceFilter();
			builder.Append(nameof(filter), filter);

			Assert.AreEqual(string.Empty, builder.ToString());
		}

		[TestMethod]
		public void Append_Ok_Default()
		{
			var builder = new QueryStringBuilder();
			var pager = new ServicePager();
			builder.Append(nameof(pager), pager);

			Assert.AreEqual(
				$"?pager.Limit={ServicePager.MaxLimit}&pager.Offset={ServicePager.MinOffset}",
				builder.ToString());
		}

		[TestMethod]
		public void Append_Ok_Array()
		{
			var builder = new QueryStringBuilder();
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			var filter = new ServiceFilter
			{
				Ids = new Guid[] { id1, id2 }
			};
			builder.Append(nameof(filter), filter);

			Assert.AreEqual(
				$"?filter.Ids={id1}&filter.Ids={id2}",
				builder.ToString());
		}

		[TestMethod]
		public void Append_Ok_Many()
		{
			var builder = new QueryStringBuilder();

			Guid id = Guid.NewGuid();
			var filter = new ServiceFilter
			{
				Ids = new Guid[] { id }
			};
			builder.Append(nameof(filter), filter);

			var pager = new ServicePager
			{
				Limit = 5,
				Offset = 1
			};
			builder.Append(nameof(pager), pager);

			Assert.AreEqual(
				$"?filter.Ids={id}&pager.Limit={pager.Limit}&pager.Offset={pager.Offset}",
				builder.ToString());
		}
	}
}
