using Microsoft.EntityFrameworkCore;
using UserSyncingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Ensure the Data project directory exists
var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../UserSyncingApp.Data");
if (!Directory.Exists(dataDirectory))
{
    Directory.CreateDirectory(dataDirectory);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseServiceStack(new AppHost());
app.Run(async context => context.Response.Redirect("/metadata"));

app.Run();