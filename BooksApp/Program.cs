using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NetCore.AutoRegisterDi;
using SqlDataLayer.SqlBookEfCore;
using SqlServiceLayer.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqlBooksConnection");
builder.Services.AddDbContext<BookSqlDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register my services
var assembliesToScan = new[]
{
    Assembly.GetExecutingAssembly(),
    Assembly.GetAssembly(typeof(ListBooksService))
};
var services = builder.Services
    .RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
    .Where(c => c.Name.EndsWith("Service"))  //optional
    .AsPublicImplementedInterfaces();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
