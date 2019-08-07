using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContructorNews.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace News.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            var claimIdentity = (ClaimsIdentity) this.User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.Users.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();

            return View("UserView", userFromDb);
        }
    }
}
