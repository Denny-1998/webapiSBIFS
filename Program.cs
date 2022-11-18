global using Microsoft.EntityFrameworkCore;
global using webapiSBIFS.Model;
using webapiSBIFS.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string conString = "Data Source=" + Environment.MachineName + ";Initial Catalog=dbSBIFS;Integrated Security=True;TrustServerCertificate=True";
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
    options.UseSqlServer(conString);
});

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
