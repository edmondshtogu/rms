using RMS.Core.Domain.RequestBC;

namespace RMS.Persistence.Ef.RequestBC
{
    public sealed class RequestStatusRepository : EfRepository<RequestStatus>, IRequestStatusRepository
    {
        public RequestStatusRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
