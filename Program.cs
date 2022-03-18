using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(o =>
    {
        // set the path for the authentication challenge
        o.LoginPath = "/login";
        // set the path for the sign out
        o.LogoutPath = "/signout";
    })
    .AddGitHub(o =>
    {
        o.ClientId = "1f0680d8244dbf122a16";
        o.ClientSecret = "72f13d82d0798ae4cd930969caf6ccdc860c3480";
        o.CallbackPath = "/signingithub";
        o.SaveTokens=true;
        // Grants access to read a user's profile data.
        // https://docs.github.com/en/developers/apps/building-oauth-apps/scopes-for-oauth-apps
        o.Scope.Add("read:user");

        // Optional
        // if you need an access token to call GitHub Apis
        o.Events.OnCreatingTicket += context =>
        {
            if (context.AccessToken is { })
            {
                context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
            }

            return Task.CompletedTask;
        };
    });
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/signout", async ctx =>
    {
        await ctx.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    });
});
app.MapRazorPages();

app.Run();
