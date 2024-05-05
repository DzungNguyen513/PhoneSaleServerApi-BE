using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PhoneSaleAPI.Firebase;
using PhoneSaleAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddCors(option => option.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));*/
builder.Services.AddCors(option => option.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://127.0.0.1:5500").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddDbContext<PhoneManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbPhoneManagement")));


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images")),
    RequestPath = "/Assets/Images"
});

app.UseCors(options =>
    options
    .WithOrigins("http://127.0.0.1:5500")
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors(options => options.WithOrigins("http://127.0.0.1:5500").AllowAnyMethod().AllowAnyHeader());
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images")),
    RequestPath = "/Assets/Images"
});

FirebaseManager.InitializeFirebaseApp();

app.Run();



