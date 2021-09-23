using System;

namespace StatusPage.Api
{
	public class InvalidServiceException : ArgumentException
	{
		public InvalidServiceException(string message) :
			base(message, (Exception)null)
		{
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