using API.Dependencies;
using Infrastructure;
using Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddJsonOptions(builder.Configuration);

builder.Services.AddInfraConnection(builder.Configuration);

builder.Services.AddCors(builder.Configuration);

builder.Services.AddAuth(builder.Configuration);

builder.Services.AddGrahQL(builder.Configuration);

builder.Services.AddSwaggerAuth(builder.Configuration);

builder.Services.AddResponseCompression();

builder.Services.AddMemoryCache();

builder.Services.AddHealthChecks();

builder.Host.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    app.UpdateDatabase(db);
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
