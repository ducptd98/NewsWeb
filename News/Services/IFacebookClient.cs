using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News.Services
{
    public interface IFacebookClient
    {
        Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null);
        Task PostAsync(string accessToken, string endpoint, object data, string args = null);
    }
}
