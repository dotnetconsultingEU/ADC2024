using adc2024.tokenprovider.Code;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Konfiguration für Jwt hinzufügen
IConfigurationSection configurationSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(configurationSection);
// ITokenService hinzufügen
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
