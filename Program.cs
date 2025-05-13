using GyanGanga.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GyanGanga.Web.Models;
using GyanGanga.Web.Models.Identity;
using GyanGanga.Web.Services;
using GyanGanga.Web.Services.Interfaces;
using GyanGanga.Web.Models.Classes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDB>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<MyDB>().AddDefaultTokenProviders();
builder.Services.AddScoped<IBookHelper, BookHelper>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MyDB>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Seed default user (jack@example.com)
    var user = await userManager.FindByEmailAsync("jack@example.com");
    if (user != null)
    {
        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, "Password123");
        user.NormalizedUserName = userManager.NormalizeName(user.UserName);
        user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        await userManager.UpdateAsync(user);
        await userManager.AddToRoleAsync(user, "User");
    }
    else
    {
        user = new ApplicationUser
        {
            UserName = "jack@example.com",
            Email = "jack@example.com",
            FirstName = "Jack",
            LastName = "Smith",
            PhoneNumber = "1234567890"
        };
        var result = await userManager.CreateAsync(user, "Password123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
        }
    }

    // Seed admin user (admin@example.com)
    var adminUser = await userManager.FindByEmailAsync("admin@example.com");
    if (adminUser != null)
    {
        await userManager.RemovePasswordAsync(adminUser);
        await userManager.AddPasswordAsync(adminUser, "Admin123!");
        adminUser.NormalizedUserName = userManager.NormalizeName(adminUser.UserName);
        adminUser.NormalizedEmail = userManager.NormalizeEmail(adminUser.Email);
        adminUser.SecurityStamp = Guid.NewGuid().ToString();
        adminUser.ConcurrencyStamp = Guid.NewGuid().ToString();
        await userManager.UpdateAsync(adminUser);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    else
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "9876543210"
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    var bookCount = await db.BookList.CountAsync();
    if (bookCount == 0)
    {
        var books = new List<Book>
        {
            new Book { BookId = 1, BookTitle = "The Great Gatsby", BookAuthor = "F. Scott Fitzgerald", BookGenre = "Fiction", BookPrice = 10.99m, BookFormat = "Paperback", BookStock = 50, BookIsbn = "978-0743273565", Rating = 4.7m },
            new Book { BookId = 2, BookTitle = "Sapiens", BookAuthor = "Yuval Noah Harari", BookGenre = "Non-Fiction", BookPrice = 15.99m, BookFormat = "Hardcover", BookStock = 30, BookIsbn = "978-0062316097", Rating = 4.8m },
            new Book { BookId = 3, BookTitle = "The Da Vinci Code", BookAuthor = "Dan Brown", BookGenre = "Mystery", BookPrice = 12.99m, BookFormat = "Ebook", BookStock = 100, BookIsbn = "978-0307474278", Rating = 4.6m },
            new Book { BookId = 4, BookTitle = "Dune", BookAuthor = "Frank Herbert", BookGenre = "Sci-Fi", BookPrice = 14.99m, BookFormat = "Paperback", BookStock = 40, BookIsbn = "978-0441172719", Rating = 4.9m }
        };
        db.BookList.AddRange(books);
        await db.SaveChangesAsync();
    }
}

app.Run();