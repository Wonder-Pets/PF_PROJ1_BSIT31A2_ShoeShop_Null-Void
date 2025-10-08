var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<WebApplication5.Interfaces.IInventoryService, WebApplication5.Implementations.InventoryService>();
builder.Services.AddScoped<WebApplication5.Interfaces.IPullOutService, WebApplication5.Implementations.PullOutService>();
builder.Services.AddScoped<WebApplication5.Interfaces.IPurchaseOrderService, WebApplication5.Implementations.PurchaseOrderService>();
builder.Services.AddScoped<WebApplication5.Interfaces.IReportService, WebApplication5.Implementations.ReportService>();

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
