using RMS.Core.Domain.RequestBC;

namespace RMS.Persistence.Ef.RequestBC
{
    public sealed class RequestRepository : EfRepository<Request>, IRequestRepository
    {
        public RequestRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
