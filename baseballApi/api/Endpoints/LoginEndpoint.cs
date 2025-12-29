using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using baseballApi.Models;

namespace baseballApi.Endpoints;

public class LoginEndpoint : BaseEndpoint
{
    public override string Name => "Login";

    public override async Task<IResult> Run(HttpContext context)
    {
        if (context.Request.Method != HttpMethods.Post) 
        {
            return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);
        }

        // リクエストボディからログイン情報を取得
        var loginReq = await context.Request.ReadFromJsonAsync<LoginRequest>();
        if (loginReq == null) return Results.BadRequest();

        // DBからユーザー取得 (QueryFirstOrDefaultAsyncを利用)
        User? user = await QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM users WHERE s_loginid = @Username", new { Username = loginReq.LoginId });

        if (user == null) return Results.Unauthorized();

        // --- パスワード比較ロジック ---
        bool isPasswordValid = false;
        if (user.s_password?.Length <= 8)
        {
            // 8文字以下の場合は生データ比較
            isPasswordValid = (user.s_password == loginReq.Password);
        }
        else
        {
            // 8文字より長い場合はハッシュ化して比較 (SHA256例)
            var hashedInput = HashPassword(loginReq.Password);
            isPasswordValid = (user.s_password == hashedInput);
        }

        if (!isPasswordValid) return Results.Unauthorized();

        // トークン生成
        var token = GenerateJwtToken(user);
        var refreshToken = Guid.NewGuid().ToString(); // 簡易的なリフレッシュトークン

        // 本来はrefreshTokenをDBに保存する処理が必要
        await ExecuteAsync("UPDATE users SET s_refresh_token = @Token WHERE n_user_id = @Id", 
            new { Token = refreshToken, Id = user.n_user_id });

        return Results.Ok(new { 
            AccessToken = token, 
            RefreshToken = refreshToken 
        });
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private string GenerateJwtToken(User user)
    {
        // Program.cs等で設定したKeyを使用
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey1234567890123456"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.s_username!),
            new Claim("UserId", user.s_loginid!)
        };

        var token = new JwtSecurityToken(
            issuer: "baseball-api",
            audience: "baseball-app",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15), // AccessTokenは短命に
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string LoginId, string Password);