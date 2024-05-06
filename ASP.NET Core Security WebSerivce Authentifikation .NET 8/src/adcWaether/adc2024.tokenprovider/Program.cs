using adc2024.tokenprovider.Code;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Konfiguration f�r Jwt hinzuf�gen
IConfigurationSection configurationSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(configurationSection);
// ITokenService hinzuf�gen
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
