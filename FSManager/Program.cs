using FSManager.Mapping;
using Microsoft.EntityFrameworkCore;

namespace FSManager;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // DB
        builder.Services.AddDbContext<CardRepository>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("CardsContext")));

        builder.Services.AddTransient<ICardRepository, CardRepository>();
        builder.Services.AddTransient<ICollectionRepository, CardRepository>();

        // services
        builder.Services.AddTransient<ICardService, CardService>();
        builder.Services.AddSingleton<IMatchService, MatchService>();

        // mapping
        builder.Services.AddAutoMapper(
            typeof(AutoMapperProfile)
        );

        builder.Services.AddSignalR();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // settings
        builder.Services.Configure<MatchesSettings>(
            builder.Configuration.GetSection("Matches")
        );
        builder.Services.Configure<CardSettings>(
            builder.Configuration.GetSection("Cards")
        );


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");

            app.UseSwagger();
            app.UseSwaggerUI();
            
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // Add WebSockets
        app.UseWebSockets(new() {
            KeepAliveInterval = TimeSpan.FromMinutes(10)
        });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}