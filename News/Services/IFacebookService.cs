using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using News.Models;

namespace News.Services
{
    public interface IFacebookService
    {
        Task<User> GetUserAsync(string accessToken);
        Task PostOnWallAsync(string accessToken, string message);
    }
}
