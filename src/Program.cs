using OpentelemetryDotnetCollector.Service;
using Microsoft.AspNetCore.Server.Kestrel.Core;


const int port = 5001;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
  // Setup a HTTP/2 endpoint without TLS.
  options.ListenLocalhost(port, o => o.Protocols =
      HttpProtocols.Http2);
});
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<TraceService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();
