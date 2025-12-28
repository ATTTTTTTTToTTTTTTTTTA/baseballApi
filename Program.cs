using System.Reflection;
using baseballApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Endpoints フォルダ内にある BaseEndpoint を継承したクラスをすべて探してインスタンス化
var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.IsSubclassOf(typeof(BaseEndpoint)) && !t.IsAbstract);

foreach (var type in endpointTypes)
{
    var endpoint = (BaseEndpoint)Activator.CreateInstance(type)!;
    endpoint.Map(app); // ルーティングの登録
}

app.Run();