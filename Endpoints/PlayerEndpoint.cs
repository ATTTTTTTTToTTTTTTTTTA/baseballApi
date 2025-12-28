using baseballApi.Models;

namespace baseballApi.Endpoints;

public class PlayerEndpoint : BaseEndpoint
{
    public override string Name => "Player";

    public override async Task<IResult> Run(HttpContext context)
    {
        var method = context.Request.Method;
        var id = context.Request.RouteValues["id"]?.ToString();

        // --- GET 処理 ---
        if (method == HttpMethods.Get)
        {
            if (string.IsNullOrEmpty(id))
            {
                // 共通メソッドで全件取得
                var players = await QueryAsync<Player>("SELECT * FROM players");
                return Results.Ok(players);
            }
            else
            {
                // 共通メソッドで1件取得
                var player = await QueryFirstOrDefaultAsync<Player>(
                    "SELECT * FROM players WHERE id = @Id", new { Id = id });

                return player != null ? Results.Ok(player) : Results.NotFound();
            }
        }

        // --- DELETE 処理 (例) ---
        if (method == HttpMethods.Delete)
        {
            var affectedRows = await ExecuteAsync(
                "DELETE FROM players WHERE id = @Id", new { Id = id });
            
            return affectedRows > 0 ? Results.NoContent() : Results.NotFound();
        }

        return Results.NotFound();
    }
}