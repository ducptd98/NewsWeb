using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using News.Models;

namespace News.Services
{
    public class FacebookService : IFacebookService
    {
        private readonly IFacebookClient _facebookClient;

        public FacebookService(IFacebookClient facebookClient)
        {
            _facebookClient = facebookClient;
        }

        public async Task<User> GetUserAsync(string accessToken)
        {
            var result = await _facebookClient.GetAsync<dynamic>(
                accessToken, "me", "fields=id,name,email,location,picture");
            if (result == null)
            {
                return new User();
            }

            var user = new User()
            {
                Id = result.id.ToString(),
                Email = result.email.ToString(),
                Name = result.name.ToString(),
                Picture = result.picture["url"].ToString(),
                Address = result.location["name"].ToString(),
                isAdmin = false
            };

            return user;
        }

        public async Task PostOnWallAsync(string accessToken, string message)
            => await _facebookClient.PostAsync(accessToken, "me/feed", new { message });
    }
}
