namespace GenericNotification.Core.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IConfigurationRepository Configuration { get; }
        ISubscriberRepository Subscriber { get; }

        Task<int> SaveAsync();

        void DisableDetectChanges();

        void EnableDetectChanges();

        int Save();
    }
}
