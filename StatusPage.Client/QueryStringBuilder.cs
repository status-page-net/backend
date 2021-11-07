using System;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;

namespace StatusPage.Client
{
	public class QueryStringBuilder
	{
		private readonly StringBuilder _builder = new StringBuilder();

		public void Append(string prefix, object obj)
		{
			Type type = obj.GetType();
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				object value = property.GetValue(obj);
				if (value == null)
				{
					continue;
				}
				var enumerable = value as IEnumerable;
				if (enumerable == null)
				{
					enumerable = new object[] { value };
				}
				foreach (object item in enumerable)
				{
					if (item == null)
					{
						continue;
					}
					if (0 < _builder.Length)
					{
						_builder.Append('&');
					}
					// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#complex-types
					_builder.Append($"{prefix}.{property.Name}=");
					_builder.Append(WebUtility.UrlEncode(item.ToString()));
				}
			}
		}

		public override string ToString()
		{
			if (_builder.Length == 0)
			{
				return string.Empty;
			}
			return "?" + _builder.ToString();
		}
	}
}