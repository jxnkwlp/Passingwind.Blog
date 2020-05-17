namespace Passingwind.Blog.Web.Models.Account
{
	public class StatusMessageViewModel
	{
		public bool Success { get; set; }
		public string Message { get; set; }


		public StatusMessageViewModel()
		{

		}

		public StatusMessageViewModel(bool success, string message)
		{
			Success = success;
			Message = message;
		}


		public static StatusMessageViewModel Succeed(string message)
		{
			return new StatusMessageViewModel()
			{
				Message = message,
				Success = true,
			};
		}

		public static StatusMessageViewModel Error(string message)
		{
			return new StatusMessageViewModel()
			{
				Message = message,
				Success = false,
			};
		}

	}
}
