using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
	public class UserForRegisterDTO
	{
        public string? FirstName { get; init; }
		public string? LastName { get; init;}

		[Required(ErrorMessage ="User name required!")]
		public string? UserName { get; init; }

		[Required(ErrorMessage = "Password required!")]
		public string? Password { get; init; }
        public string? Email { get; init; }
		public string? PhoneNumber { get; init; }
        public ICollection<string> Roles { get; init; }
    }
}
