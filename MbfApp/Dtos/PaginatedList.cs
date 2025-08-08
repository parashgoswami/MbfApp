using Microsoft.EntityFrameworkCore;

namespace MbfApp.Dtos
{
    public class PaginatedList<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public PaginatedList(IReadOnlyList<T> results, int pageCount, int totalCount)
        {
            Items = results;
            TotalPages = pageCount;
            TotalCount = totalCount;
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int? pageIndex, int? pageSize)
        {
            var pageNumber = pageIndex ?? 1;
            var pageLength = pageSize ?? 10;

            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageLength).Take(pageLength).ToListAsync();

            return new PaginatedList<T>(items, (int)Math.Ceiling(count / (double)pageLength), count);
        }
    }
}