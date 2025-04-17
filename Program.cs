using Microsoft.EntityFrameworkCore; 
using GyanGanga.Web.Data; 
using GyanGanga.Web.Services; 
using GyanGanga.Web.Services.Interfaces; 
 
var builder = WebApplication.CreateBuilder(args); 
 
// Add MVC services 
builder.Services.AddControllersWithViews(); 
 
// Configure SQLite database 
builder.Services.AddDbContext<GyanGangaDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); 
 
// Add AutoMapper for model mapping 
builder.Services.AddAutoMapper(typeof(Program)); 
 
// Register custom services 
builder.Services.AddScoped<IBookService, BookService>(); 
 
var app = builder.Build(); 
 
// Configure the HTTP pipeline 
if (!app.Environment.IsDevelopment()) 
{ 
    app.UseExceptionHandler("/Home/Error"); 
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
