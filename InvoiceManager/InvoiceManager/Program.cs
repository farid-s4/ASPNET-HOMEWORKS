using InvoiceManager.Data;
using InvoiceManager.Extensions;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMySwagger()
    .AddDbInvoiceManagerContext(builder.Configuration)
    .AddAuthAndJwt(builder.Configuration)
    .AddAutoMapperAndServices()
    .AddFluentValidation();

QuestPDF.Settings.License = LicenseType.Community;
var app = builder.Build();

app.UsePipeline();

await app.SeedRoles("Admin", "User", "Manager");

app.Run();