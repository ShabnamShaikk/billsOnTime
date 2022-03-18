using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Project1.Helpers;
using System.Security.Claims;

namespace Project1.Pages
{
    [Authorize]
    public class AccountDetailModel : PageModel
    {

        public async Task OnGet()
        {
            
            Claims = User.Claims.ToList();

            if (User.AccessToken() is { } accessToken)
            {
                var client = new GitHubClient(new ProductHeaderValue("test"))
                {
                    Credentials = new Credentials(accessToken)
                };
                GitHubUser = await client.User.Get(User.Identity?.Name);
            }
        }

        public User GitHubUser { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
