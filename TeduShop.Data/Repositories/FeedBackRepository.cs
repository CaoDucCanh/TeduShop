using TeduShop.Data.Infrastructure;
using TeduShop.Model.Models;

namespace TeduShop.Data.Repositories
{
    public interface IFeedBackDetailRepository : IRepository<Feedback>
    {
    }

    public class FeedBackRepository : RepositoryBase<Feedback>, IFeedBackDetailRepository
    {
        public FeedBackRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}