using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace ManningBooksApiSecure.Tests;

public class LogSqlInterceptor : DbCommandInterceptor
{
    private readonly ITestOutputHelper _testOutput;

    public LogSqlInterceptor(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken token
    )
    {
        _testOutput.WriteLine(command.CommandText.Trim());
        return base.ReaderExecutingAsync(command, eventData, result, token);
    }
}
