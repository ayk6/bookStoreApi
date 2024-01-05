using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
	public record UserForAuthDTO
	{
		[Required(ErrorMessage = "User name required!")]
		public string? UserName { get; init; }

		[Required(ErrorMessage = "Password required!")]
		public string? Password { get; init; }
	}
}
