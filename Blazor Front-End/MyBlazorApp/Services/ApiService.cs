using Microsoft.Extensions.Configuration;
using System.IO;

namespace MyBlazorApp.Services
{
    public class ApiService
    {
        private readonly IConfiguration _configuration;
        
        // Constructor: Inject IConfiguration to access appsettings
        public ApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // Method: Access the BaseUrl from appsettings.Development.json
        public string GetSetting(string key)
        {
            return _configuration["ApiSettings:BaseUrl"] ?? string.Empty;
        }
    }
}