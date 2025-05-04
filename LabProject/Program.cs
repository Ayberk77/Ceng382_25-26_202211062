using LabProject.Data;
using LabProject.Models;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); 
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

    // Eğer hiç kullanıcı yoksa 1 admin user ekle
    if (!context.Users.Any())
    {
        context.Users.Add(new User
        {
            Username = "admin",
            Password = "admin123",
            Role = "admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    // Eğer hiç sınıf yoksa örnek sınıflar ekle
    if (!context.Classes.Any())
    {
        for (int i = 1; i <= 20; i++)
        {
            context.Classes.Add(new Class
            {
                Name = $"Sample Class {i}",
                PersonCount = new Random().Next(10, 40),
                Description = $"Seeded description {i}",
                IsActive = true
            });
        }
    }

    context.SaveChanges();
}



app.Run();
