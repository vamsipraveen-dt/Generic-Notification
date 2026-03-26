using GenericNotification.Core.Domain.Models;
using GenericNotification.Core.Domain.Repositories;

namespace GenericNotification.Core.Persistence.Repositories
{
    public class UnitOfWork(GenericNotificationContext context, IConfigurationRepository configurationRepository,
        ISubscriberRepository subscriberRepository) : IUnitOfWork
    {
        private readonly GenericNotificationContext _context = context;

        public IConfigurationRepository Configuration { get; } = configurationRepository;
        public ISubscriberRepository Subscriber { get; } = subscriberRepository;

        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();

        public int Save()
            => _context.SaveChanges();

        public void DisableDetectChanges()
            => _context.ChangeTracker.AutoDetectChangesEnabled = false;

        public void EnableDetectChanges()
            => _context.ChangeTracker.AutoDetectChangesEnabled = true;
    }
}
