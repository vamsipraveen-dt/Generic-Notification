using GenericNotification.Core.Domain.Models;
using GenericNotification.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GenericNotification.Core.Persistence.Repositories
{
    public class SubscriberRepository : GenericRepository<Subscriber, Ra02Context>,
        ISubscriberRepository
    {
        private readonly ILogger<SubscriberRepository> _logger;
        public SubscriberRepository(Ra02Context context,
            ILogger<SubscriberRepository> logger) : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<Subscriber?> GetSubscriberInfo(Subscriber SubInfo)
        {
            try
            {
                return await Context.Subscribers.AsNoTracking().SingleOrDefaultAsync(ss => ss.SubscriberUid == SubInfo.SubscriberUid);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfo::Database exception");
                return null;
            }
        }

        public async Task<Subscriber?> GetSubscriberInfoByEmail(string emailId)
        {
            try
            {
                return await Context.Subscribers.AsNoTracking().Include(s => s.SubscriberFcmToken).SingleOrDefaultAsync(ss => ss.EmailId == emailId);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfoByEmail::Database exception");
                throw;
            }
        }


        //public async Task<IList<Subscriber>> GetSubscriberInfoByOrgnizationEmail(string emailId)
        //{
        //	try
        //	{
        //		return await Context.Subscribers.Where(ss => ss.OrgEmailsList.ToLower().Contains(emailId.ToLower())).AsNoTracking().ToListAsync();
        //	}
        //	catch (Exception error)
        //	{
        //		_logger.LogError("GetSubscriberInfoByEmail::Database exception {0}", error);
        //		throw;
        //	}
        //}

        public async Task<Subscriber?> GetSubscriberInfoByPhone(string phoneNo)
        {
            try
            {
                return await Context.Subscribers.AsNoTracking().SingleOrDefaultAsync(ss => ss.MobileNumber == phoneNo);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfoByPhone::Database exception");
                throw;
            }
        }

        public async Task<Subscriber?> GetSubscriberInfoBySUID(string Suid)
        {
            try
            {
                return await Context.Subscribers.AsNoTracking().Include(s => s.SubscriberFcmToken).SingleOrDefaultAsync(ss => ss.SubscriberUid == Suid);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfoBySUID::Database exception");
                throw;
            }
        }

        public async Task<List<Subscriber>> GetSubscriberInfoBySUIDList(List<string> SuidList)
        {
            try
            {
                return await Context.Subscribers.Where<Subscriber>(ss => SuidList.Contains(ss.SubscriberUid)).AsNoTracking().Include(s => s.SubscriberFcmToken).ToListAsync();
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfoBySUIDList::Database exception");
                throw;
            }
        }

        public async Task<Subscriber?> GetSubscriberInfobyDocType(string docNumber)
        {
            try
            {
                return await Context.Subscribers.AsNoTracking().Include(s => s.SubscriberFcmToken).SingleOrDefaultAsync(ss => ss.IdDocNumber == docNumber);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetSubscriberInfobyDocType::Database exception");
                return null;
            }
        }
    }
}
