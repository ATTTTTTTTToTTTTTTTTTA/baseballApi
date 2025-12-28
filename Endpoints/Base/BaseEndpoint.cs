using Dapper;
using MySqlConnector;

namespace baseballApi.Endpoints;

public abstract class BaseEndpoint
{
    public abstract string Name { get; }
    protected ILogger? Logger { get; private set; }
    private string? ConnectionString { get; set; }

    public void Map(IEndpointRouteBuilder app)
    {
        var route = $"/{Name.ToLower()}";
        var routeWithId = $"{route}/{{id}}";
        var methods = new[] { "GET", "POST", "PUT", "DELETE" };

        app.MapMethods(route, methods, async (HttpContext context) => {
            InitServices(context);
            return await SafeRun(context);
        });
        app.MapMethods(routeWithId, methods, async (HttpContext context) => {
            InitServices(context);
            return await SafeRun(context);
        });
    }

    private void InitServices(HttpContext context)
    {
        var factory = context.RequestServices.GetRequiredService<ILoggerFactory>();
        Logger = factory.CreateLogger(this.GetType());
        var config = context.RequestServices.GetRequiredService<IConfiguration>();
        ConnectionString = config.GetConnectionString("DefaultConnection");
    }

    // --- 共通クエリ実行メソッド ---

    // 取得系 (SELECT)
    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(Query): {Sql} | Params: {@Param}", sql, param);
        return await connection.QueryAsync<T>(sql, param);
    }

    // 1件取得系 (SELECT TOP 1)
    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(QueryFirst): {Sql} | Params: {@Param}", sql, param);
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    // 更新系 (INSERT, UPDATE, DELETE)
    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(Execute): {Sql} | Params: {@Param}", sql, param);
        return await connection.ExecuteAsync(sql, param);
    }

    private async Task<IResult> SafeRun(HttpContext context)
    {
        try
        {
            return await Run(context);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "API {Name} で例外が発生しました。", Name);
            return Results.Problem("システムエラーが発生しました。");
        }
    }

    public abstract Task<IResult> Run(HttpContext context);
}