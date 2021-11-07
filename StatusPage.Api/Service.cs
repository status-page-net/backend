using System;

namespace StatusPage.Api
{
	[Serializable]
	public class Service
	{
		/// <summary>
		/// Unique identifier of the Service.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Unique identifier of Service's state.
		/// ETag protects the object from the Lost Update problem.
		/// </summary>
		public Guid ETag { get; set; }

		/// <summary>
		/// Title of the Service.
		/// </summary>
		public string Title { get; set; }

		public override bool Equals(object obj)
		{
			Service service = obj as Service;
			if (service == null)
			{
				return false;
			}
			return (Id == service.Id)
				&& (ETag == service.ETag)
				&& (Title == service.Title);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode()
				^ ETag.GetHashCode();
		}

		public Service Clone(bool refreshETag)
		{
			var clone = MemberwiseClone() as Service;
			if (refreshETag)
			{
				clone.ETag = Guid.NewGuid();
			}
			return clone;
		}
	}

	public static class ServiceExt
	{
		/// <summary>
		/// Validates the Service.
		/// </summary>
		/// <exception cref="ApiArgumentException" />
		public static void Validate(this Service service)
		{
			if (service == null)
			{
				throw new ApiArgumentException(typeof(Service));
			}
			if (service.Title == null)
			{
				throw new ApiArgumentException(typeof(Service), nameof(Service.Title));
			}
		}
	}
}