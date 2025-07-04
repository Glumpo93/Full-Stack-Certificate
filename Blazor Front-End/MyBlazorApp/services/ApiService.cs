using Microsoft.Extensions.Configuration;

public class ApiService
    {
        private readonly IConfiguration _configuration;

        public ApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetApiUrl()
        {
            return _configuration["ApiSettings:BaseUrl"];
        }
}
