using RMS.Core.Domain;
using System.Threading.Tasks;

namespace RMS.Messages
{
    public interface IQueryHandler<in TQuery, TQueryResult>
        where TQuery : IQuery<TQueryResult>
    {
        Task<Result<TQueryResult>> HandleAsync(TQuery queryModel);
    }
}