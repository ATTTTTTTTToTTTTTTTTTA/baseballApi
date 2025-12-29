namespace baseballApi.Endpoints;

public class RefreshEndpoint : BaseEndpoint
{
    public override string Name => "Refresh";

    public override async Task<IResult> Run(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Post) 
        {
            return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        var req = await context.Request.ReadFromJsonAsync<RefreshRequest>();
        
        // DBからリフレッシュトークンが一致するユーザーを検索
        var user = await QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM users WHERE refresh_token = @Token", new { Token = req?.RefreshToken });

        if (user == null) return Results.Unauthorized();

        // 新しいトークンを生成（LoginEndpointと同様のロジック）
        // ※ 共通化して別のServiceクラスに切り出すのが理想的です
        return Results.Ok(new { AccessToken = "NEW_JWT_TOKEN" });
    }
}

public record RefreshRequest(string RefreshToken);