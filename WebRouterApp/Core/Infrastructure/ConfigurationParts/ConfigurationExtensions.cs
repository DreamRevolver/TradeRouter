using Microsoft.Extensions.Configuration;

namespace WebRouterApp.Core.Infrastructure.ConfigurationParts
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetJwtSection(this IConfiguration configuration)
        {
            var section = configuration.GetSection("Jwt");
            return section;
        }

        public static IConfigurationSection GetRefreshTokenSection(this IConfiguration configuration)
        {
            var section = configuration.GetSection("RefreshToken");
            return section;
        }

        public static IConfigurationSection GetAppSettingsSection(this IConfiguration configuration)
        {
            var section = configuration.GetSection("AppSettings");
            return section;
        }

        public static string GetAppSetting(this IConfiguration configuration, string key)
        {
            var appSettings = configuration.GetAppSettingsSection();
            var value = appSettings[key];
            return value;
        }
    }
}
