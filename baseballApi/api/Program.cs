using System.Reflection;
using baseballApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// 1. CORSポリシーを定義
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // VueのURLを許可
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();

// 2. RoutingとAuthの間に挿入
app.UseCors("MyAllowSpecificOrigins");

// Endpoints フォルダ内にある BaseEndpoint を継承したクラスをすべて探してインスタンス化
var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.IsSubclassOf(typeof(BaseEndpoint)) && !t.IsAbstract);

foreach (var type in endpointTypes)
{
    var endpoint = (BaseEndpoint)Activator.CreateInstance(type)!;
    endpoint.Map(app); // ルーティングの登録
}

app.Run();