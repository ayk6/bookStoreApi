using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;

namespace Services
{
	public class AuthManager : IAuthService
	{
		private readonly ILoggerService _loggerService;
		private readonly IMapper _mapper;
		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _config;

		private User? _user;

		public AuthManager(ILoggerService loggerService, IMapper mapper, UserManager<User> userManager, IConfiguration config)
		{
			_loggerService = loggerService;
			_mapper = mapper;
			_userManager = userManager;
			_config = config;
		}

		public async Task<TokenDTO> CreateToken(bool populateExp)
		{
			var signingCredentials = GetSigningCredentials();
			var claims = await getClaims();
			var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

			var refreshToken = GenerateRefreshToken();
			_user.RefreshToken = refreshToken;

			if (populateExp)
			{
				_user.RefreshTokenExpireTime = DateTime.Now.AddDays(1);
			}
			await _userManager.UpdateAsync(_user);

			var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
			return new TokenDTO()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
			};
		}

		public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
		{
			var principal = GetPrincipalFromExpToken(tokenDTO.AccessToken);
			var user = await _userManager.FindByNameAsync(principal.Identity.Name);

			if (user is null || user.RefreshToken != tokenDTO.RefreshToken ||
				user.RefreshTokenExpireTime <= DateTime.Now)
			{
				throw new RefreshTokenBadRequestException();
			}
			_user = user;
			return await CreateToken(populateExp: false);
		}

		public async Task<IdentityResult> RegisterUserAsync(UserForRegisterDTO userForRegisterDTO)
		{
			var user = _mapper.Map<User>(userForRegisterDTO);
			var result = await _userManager.CreateAsync(user, userForRegisterDTO.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRolesAsync(user, userForRegisterDTO.Roles);
			}
			return result;
		}

		public async Task<bool> ValidateUser(UserForAuthDTO userForAuthDTO)
		{
			_user = await _userManager.FindByNameAsync(userForAuthDTO.UserName);
			var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthDTO.Password));
			if (!result)
			{
				_loggerService.LogWarning($"{nameof(ValidateUser)} : Authentication failed");
			}
			return result;
		}

		private SigningCredentials GetSigningCredentials()
		{
			var jwtSettings = _config.GetSection("JwtSettings");
			var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
			var secret = new SymmetricSecurityKey(key);

			return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
		}
		private async Task<List<Claim>> getClaims()
		{
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, _user.UserName)
			};
			var roles = await _userManager.GetRolesAsync(_user);
			foreach (var role in roles) 
			{ 
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			return claims;
		}
		private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
		{
			var jwtSettings = _config.GetSection("JwtSettings");
			var tokenOptions = new JwtSecurityToken(
					issuer: jwtSettings["validIssuer"],
					audience: jwtSettings["validAudience"],
					claims,
					expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
					signingCredentials: signingCredentials);

			return tokenOptions;
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
		private ClaimsPrincipal GetPrincipalFromExpToken(string exptoken)
		{
			var jwtSettings = _config.GetSection("JwtSettings");
			var secretKey = jwtSettings["secretKey"];
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings["validIssuer"],
				ValidAudience = jwtSettings["validAudience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(exptoken, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase))
            {
				throw new SecurityTokenException("invalid token");
            }
            return principal;
		}
	}
}
