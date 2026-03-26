using GenericNotification.Core.Domain.Models;

namespace GenericNotification.Core.Domain.Repositories
{
    public interface ISubscriberRepository
    {
        Task<Subscriber?> GetSubscriberInfo(Subscriber SubInfo);
        Task<Subscriber?> GetSubscriberInfoByEmail(string emailId);
        Task<Subscriber?> GetSubscriberInfoByPhone(string phoneNo);
        Task<Subscriber?> GetSubscriberInfoBySUID(string Suid);
        Task<Subscriber?> GetSubscriberInfobyDocType(string docNumber);

        Task<List<Subscriber>> GetSubscriberInfoBySUIDList(List<string> SuidList);
    }
}
