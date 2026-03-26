using GenericNotification.Core.Domain.Models;
using GenericNotification.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GenericNotification.Core.Persistence.Repositories
{
    public class ConfigurationRepository : GenericRepository<Configuration, GenericNotificationContext>,
        IConfigurationRepository
    {
        private readonly ILogger<ConfigurationRepository> _logger;
        public ConfigurationRepository(GenericNotificationContext context,
            ILogger<ConfigurationRepository> logger) : base(context, logger)
        {
            _logger = logger;
        }

        public Configuration? GetConfigurationByName(string name)
        {
            try
            {
                return Context.Configurations.FirstOrDefault(c => c.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetConfigurationByName:: Database exception");
                //Monitor.SendException(ex);
                return null;
            }
        }

        public async Task<Configuration?> GetConfigurationByNameAsync(string name)
        {
            try
            {
                return await Context.Configurations.FirstOrDefaultAsync(c => c.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetConfigurationByName:: Database exception");
                //Monitor.SendException(ex);
                return null;
            }
        }
    }
}
