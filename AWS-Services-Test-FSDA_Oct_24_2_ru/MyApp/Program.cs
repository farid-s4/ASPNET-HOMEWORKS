using Amazon.Runtime;
using Amazon.S3;
using MyApp.Data;
using MyApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("ReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://fsdaoct242ruproductsitebucket.s3-website.eu-north-1.amazonaws.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
    );

builder.Services.AddSingleton<IAmazonS3>(
    _ =>
    {
        var awsSection = builder.Configuration.GetSection("AWS");
        var settings = awsSection.Get<AWSSettings>();

        var region = Amazon.RegionEndpoint.GetBySystemName(settings!.Region);
        if (!string.IsNullOrWhiteSpace(settings.AccessKey) && !string.IsNullOrWhiteSpace(settings.SecretKey))
        {
            var credentials = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
            return new AmazonS3Client(credentials, region);
        }
        return new AmazonS3Client(region);
    }

    );

builder.Services.AddScoped<IStorage, S3Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactApp");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    await DbSeeder.SeedAsync(dbContext);
}
app.Run();
