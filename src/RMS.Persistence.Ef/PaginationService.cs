using RMS.Core;
using RMS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace RMS.Persistence.Ef
{
    public class PaginationService : IPaginationService
    {
        private readonly ITypeAdapter _typeAdapter;
        private readonly PaginationOptions _options;

        public PaginationService(ITypeAdapter typeAdapter, PaginationOptions options)
        {
            _typeAdapter = typeAdapter;
            _options = options;
        }

        public PaginatedItemsResult<TItemResult> Paginate<TItemResult, TSource>(
            IQueryable<TSource> source,
            string orderByString,
            int pageIndex,
            int pageSize,
            Func<TSource, bool> filter = null)
            where TItemResult : class
            where TSource : BaseEntity
        {
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }

            pageSize = pageSize == 0 ? _options.DefaultPageSize : Math.Min(pageSize, _options.MaxPageSizeAllowed);

            var count = source.LongCount();

            if (filter != null)
            {
                source = source.Where(filter).AsQueryable();
            }

            var filteredCount = source.Count();

            var data = source.OrderBy(orderByString)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedItemsResult<TItemResult>
                (
                    _typeAdapter.Adapt<IEnumerable<TItemResult>>(data),
                    (int)Math.Floor((decimal)count / pageSize),
                    count,
                    filteredCount
                );
        }
    }

    public class PaginationOptions
    {
        /// <summary>
        /// Default is 20
        /// </summary>
        public int MaxPageSizeAllowed { get; set; } = 20;

        public int DefaultPageSize { get; set; } = 20;
    }
}
