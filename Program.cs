global using Microsoft.EntityFrameworkCore;
global using webapiSBIFS.Model;
global using webapiSBIFS.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using webapiSBIFS.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Service for fixing possible object cycles
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Database and salt setup ensurance

string conString0 = builder.Configuration.GetConnectionString("DefaultConnection");
//string conString1 = "Data Source=" + Environment.MachineName + ";Initial Catalog=dbSBIFS;Integrated Security=True;TrustServerCertificate=True";
//string conString2 = "Server=sql.dennym.de;Database=dbSBIFS;User Id=sa;Password=Baum876543; TrustServerCertificate=True";
//string conString3 = "Server=sql.dennym.de;Initial Catalog=dbSBIFS;Persist Security Info=False;User  ID=sa;Password=Baum876543; MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;";



FileAdapter fileTxt = new TextFile();
string configPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\SBIFS";

if (!Directory.Exists(configPath))
{
    Directory.CreateDirectory(configPath);
}
string saltPath = configPath + "\\salt.txt";
if (!File.Exists(saltPath))
{ 
    fileTxt.WriteTextToFile(saltPath, SecurityTools.GenerateSalt());
}

// Add DB Context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(conString0);
});

// UserService service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Token authentication with bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new TextFile().GetAllTextFromFile(saltPath))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseDefaultFiles();
//app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
