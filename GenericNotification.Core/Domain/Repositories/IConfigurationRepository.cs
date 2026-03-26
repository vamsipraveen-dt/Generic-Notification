using GenericNotification.Core.Domain.Models;

namespace GenericNotification.Core.Domain.Repositories
{
    public interface IConfigurationRepository : IGenericRepository<Configuration>
    {
        public Configuration? GetConfigurationByName(string name);
        public Task<Configuration?> GetConfigurationByNameAsync(string name);
    }
}
