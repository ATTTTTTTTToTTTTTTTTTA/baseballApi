using Dapper;
using MySqlConnector;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace baseballApi.Endpoints;

public abstract class BaseEndpoint
{
    public abstract string Name { get; }
    protected ILogger? Logger { get; private set; }
    private string? ConnectionString { get; set; }

    // JWTの設定（本来はappsettings.jsonから取得すべき値）
    private const string JwtSecret = "YourSuperSecretKey1234567890123456";

    public void Map(IEndpointRouteBuilder app)
    {
        var route = $"baseballapi/{Name.ToLower()}";
        var routeWithId = $"{route}/{{id}}";
        var methods = new[] { "GET", "POST", "PUT", "DELETE" };

        app.MapMethods(route, methods, async (HttpContext context) => {
            context.Request.EnableBuffering(); // ボディの複数回読み取りを許可
            InitServices(context);
            return await SafeRun(context);
        });
        app.MapMethods(routeWithId, methods, async (HttpContext context) => {
            context.Request.EnableBuffering();
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

    // --- トークンチェック (認可) ロジック ---
    private bool IsAuthorized(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) return false;

        var token = authHeader.Substring("Bearer ".Length);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(JwtSecret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "baseball-api",
                ValidateAudience = true,
                ValidAudience = "baseball-app",
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch (Exception ex)
        {
            Logger?.LogWarning("トークン検証失敗: {Message}", ex.Message);
            return false;
        }
    }

    private async Task<IResult> SafeRun(HttpContext context)
    {
        try
        {
            // Login と Refresh 以外はトークンを必須にする
            if (Name != "Login" && Name != "Refresh")
            {
                if (!IsAuthorized(context))
                {
                    Logger?.LogWarning("未認証アクセスを拒否しました: {Name}", Name);
                    return Results.Unauthorized();
                }
            }
            return await Run(context);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "API {Name} で例外が発生しました。", Name);
            return Results.Problem("システムエラーが発生しました。");
        }
    }

    // --- 共通クエリ実行メソッド (MySqlConnectionを明示的に使用) ---

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(Query): {Sql}", sql);
        return await connection.QueryAsync<T>(sql, param);
    }

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(QueryFirst): {Sql}", sql);
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        using var connection = new MySqlConnection(ConnectionString);
        Logger?.LogInformation("SQL実行(Execute): {Sql}", sql);
        return await connection.ExecuteAsync(sql, param);
    }

    public abstract Task<IResult> Run(HttpContext context);
}