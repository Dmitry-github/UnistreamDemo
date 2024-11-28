namespace UnistreamDemo.WebApi
{
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Models;

    public static class AppConfig
    {
        private static IConfiguration Configuration
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();
                return config;
            }
        }

        public static List<Client> Clients()
        {
            return Configuration.GetSection("AppSettings:ClientsSetup").Get<List<Client>>();
        }
    }
}
