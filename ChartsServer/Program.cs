using ChartsServer.Hubs;
using ChartsServer.Models;
using ChartsServer.Subscriptions;
using ChartsServer.Subscriptions.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
        policy.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true)
));

builder.Services.AddSignalR();
builder.Services.AddSingleton<DatabaseSubscription<Sale>>();
builder.Services.AddSingleton<DatabaseSubscription<Employee>>();

var app = builder.Build();

app.UseCors();

app.UseDatabaseSubscription<DatabaseSubscription<Sale>>("Sales");
app.UseDatabaseSubscription<DatabaseSubscription<Employee>>("Employees");

app.MapGet("/", () => "Hello World!");
app.MapHub<SaleHub>("/salehub");

app.Run();
