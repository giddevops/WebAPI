using System;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GidIndustrial.Gideon.WebApi.Controllers
{
	public class UserControllerHelper
	{
		public async Task<User> GetUserById(int userId, AppDBContext context)
		{
			var user = await context.Users
			.Include(item => item.DefaultGidLocationOption)
			.Include(item => item.UserGroups)
				.ThenInclude(item => item.UserGroup)
			.Include(item => item.LeadRoutingRules)
			.SingleOrDefaultAsync(m => m.Id == userId);

			return user;
		}
	}
}
