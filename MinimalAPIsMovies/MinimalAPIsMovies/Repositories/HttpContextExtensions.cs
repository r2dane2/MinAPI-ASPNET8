using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPIsMovies.Repositories;

public static class HttpContextExtensions
{
    public static async Task InsertPaginationParameterInResponseHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        double count = await queryable.CountAsync();
        httpContext.Response.Headers.Append("totalAmountOfRecords", count.ToString(CultureInfo.CurrentCulture));
    }
}