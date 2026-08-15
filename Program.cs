using AspNetStatic;
using AspNetStaticContrib.AspNetStatic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IStaticResourcesInfoProvider>(
    new StaticResourcesInfoProvider()
        .AddAllProjectRazorPages(builder.Environment)
        .AddAllWebRootContent(builder.Environment));

var app = builder.Build();
var isStaticGeneration = args.Contains("--generate-static");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

// Bypasses HTTPS redirection when exporting static files so AspNetStatic can scrape localhost pages.
if (!isStaticGeneration) {
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

if (isStaticGeneration)
{
    var outputPath = Path.Combine(app.Environment.ContentRootPath, "docs");
    Directory.CreateDirectory(outputPath);
    app.GenerateStaticContent(outputPath, alwaysDefaultFile: true);
    return;
}

app.Run();
