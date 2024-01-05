using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
	public class User : IdentityUser
	{
        public String? FirstName { get; set; }
		public String? LastName { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
    }
}
