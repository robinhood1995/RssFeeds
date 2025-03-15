using Microsoft.EntityFrameworkCore;
using RssFeeds.Components;
using RssFeeds.Models;
using RssFeeds.Repositories.Contracts;
using RssFeeds.Services;

var builder = WebApplication.CreateBuilder(args);

//https://stackoverflow.com/questions/65022729/use-both-adddbcontextfactory-and-adddbcontext-extension-methods-in-the-same
builder.Services.AddDbContextFactory<NewsFeedContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    , ServiceLifetime.Scoped);

builder.Services.AddHostedService<RssFeedBackgroundService>();
builder.Services.AddScoped<IRssFeedProcessor, RssFeedProcessor>();
builder.Services.AddHttpClient(); // Add HttpClient


// Add services to the container.
builder.Services.AddRazorPages();                      // For Razor Pages
builder.Services.AddServerSideBlazor();                // For Blazor Server
//builder.Services.AddHttpClient<RssFeedService>();      // Register RssFeedService with HttpClient

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.MapBlazorHub();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
