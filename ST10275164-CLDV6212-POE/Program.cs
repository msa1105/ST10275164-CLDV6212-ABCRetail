using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using ST10275164_CLDV6212_POE.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get the connection string
var connectionString = builder.Configuration.GetSection("AzureStorage")["ConnectionString"];

// Register Azure Clients
builder.Services.AddSingleton(x => new TableServiceClient(connectionString));
builder.Services.AddSingleton(x => new BlobServiceClient(connectionString));
builder.Services.AddSingleton(x => new QueueServiceClient(connectionString));
builder.Services.AddSingleton(x => new ShareServiceClient(connectionString));

// Register our custom services
builder.Services.AddSingleton<ITableStorageService, TableStorageService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<IQueueStorageService, QueueStorageService>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
