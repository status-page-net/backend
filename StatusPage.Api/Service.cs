using System;

namespace StatusPage.Api
{
	[Serializable]
	public class Service
	{
		public Guid Id { get; set; }
		public string Title { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is not Service service)
			{
				return false;
			}
			return (Id == service.Id)
				&& (Title == service.Title);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public Service Clone()
		{
			return MemberwiseClone() as Service;
		}

		/// <summary>
		/// Validates a service.
		/// </summary>
		/// <exception cref="InvalidServiceException" />
		public static void Validate(Service service)
		{
			if (service == null)
			{
				throw new InvalidServiceException($"{nameof(Service)} is null.");
			}
			if (service.Title == null)
			{
				throw new InvalidServiceException($"{nameof(Title)} is null.");
			}
		}
	}
}