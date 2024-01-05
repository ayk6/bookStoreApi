using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts
{
	public interface IAuthService
	{
		Task<IdentityResult> RegisterUserAsync(UserForRegisterDTO userForRegisterDTO);

		Task<bool> ValidateUser(UserForAuthDTO userForAuthDTO);

		Task<TokenDTO> CreateToken(bool populateExp);

		Task<TokenDTO> RefreshToken(TokenDTO tokenDTO);
	}
}
