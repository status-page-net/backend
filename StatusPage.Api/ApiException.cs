using System;

namespace StatusPage.Api
{
	public class ApiArgumentException : Exception
	{
		public ApiArgumentException(Type type) :
			base($"Bad argument {type.Name}")
		{
		}

		public ApiArgumentException(Type type, string field) :
			base($"Bad argument {type.Name}.{field}")
		{
		}

		public ApiArgumentException(string message) :
			base(message)
		{
		}
	}

	public class OutdatedServiceException : ArgumentException
	{
		public readonly Service Latest;

		public OutdatedServiceException(Service latest) :
			base($"Service is outdated", (Exception)null)
		{
			Latest = latest ?? throw new ArgumentNullException(nameof(latest));
		}
	}

	public class ServiceAlreadyExistsException : ArgumentException
	{
		public ServiceAlreadyExistsException(Guid id, Exception inner) :
			base($"Service already exists: {id}", inner)
		{
			Id = id;
		}

		public Guid Id { get; protected set; }
	}
}