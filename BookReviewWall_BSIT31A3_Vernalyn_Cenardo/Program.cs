using BookReviewWall_BSIT31A3_Vernalyn_Cenardo.Data; // Para sa ApplicationDbContext mo
using BookReviewWall.Infrastructure.Data; // Assuming nilikha mo na ang BookReviewDbContext dito
using BookReviewWall.Services.Implementations;
using BookReviewWall.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// IDENTITY DATABASE (In-Memory)
// ApplicationDbContext is used for Identity Tables (Users, Roles, etc.)
var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString)); // Pinalitan ko ng UseSqlServer kung sakaling SQL Server gamit mo

// BOOK REVIEW DATABASE (In-Memory - Separate)
// Assuming you have BookReviewDbContext in Infrastructure/Data
builder.Services.AddDbContext<BookReviewDbContext>(options =>
    options.UseInMemoryDatabase("BookReviewDataDb")); // Gamitin muna natin ang In-Memory para sa Books/Reviews

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity with CUSTOM PASSWORD RULES
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Register SERVICES for Dependency Injection
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // For Identity pages

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // --- TEMPORARY: Seed data initialization for in-memory DB ---
    // If you are using UseInMemoryDatabase, this seeds the initial data.
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<BookReviewDbContext>();
        // I-Seed natin ang data dito (sa huling phase)
        // DbInitializer.Initialize(context); 
    }
    // --- END TEMPORARY ---
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // IMPORTANT for Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}");
app.MapRazorPages(); // For Identity pages

app.Run();
