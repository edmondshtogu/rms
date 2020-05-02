using RMS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RMS.Core
{
    public interface IPaginationService
    {
        /// <summary>
        /// Get a IQueryable source and execute apply pagination against it.
        /// It queries DB two times, first time to get the totatl count of elements
        /// and the second time to get items based on pageSize and PageIndex.
        /// After queries are executed a mapping oepration will be executed
        /// </summary>
        /// <typeparam name="TItemResult">The type of the item result.</typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Current page index (0 based)</param>
        /// <param name="pageSize">Number of rows per page</param>
        /// <returns></returns>
        PaginatedItemsResult<TItemResult> Paginate<TItemResult, TSource, TKey>(
            IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> orderBy,
            int pageIndex,
            int pageSize) 
            where TItemResult : class
            where TSource : BaseEntity;
    }

    public class PaginatedItemsResult<TItemResult> where TItemResult : class
    {
        public PaginatedItemsResult(IEnumerable<TItemResult> data, int totalPages, long count)
        {
            TotalPages = totalPages;
            Count = count;
            Data = data;
        }

        public int TotalPages { get; private set; }
        public long Count { get; private set; }
        public IEnumerable<TItemResult> Data { get; private set; }
    }
}
