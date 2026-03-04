using InvoiceManager.Data;
using InvoiceManager.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMySwagger()
    .AddDbInvoiceManagerContext(builder.Configuration)
    .AddAuthAndJwt(builder.Configuration)
    .AddAutoMapperAndServices()
    .AddFluentValidation();

var app = builder.Build();

app.UsePipeline();

await app.SeedRoles("Admin", "User", "Manager");

app.Run();