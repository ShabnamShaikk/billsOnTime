using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Authentication;
using Octokit;
using System.Security.Claims;
using Project1.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Project1.Pages.Account
{
    public class LoginModel : PageModel
    {
        private IMongoCollection<LoginDetails> _UserLogin;

        public IEnumerable<AuthenticationScheme> Schemes { get; set; }

        public User GitHubUser { get; set; }
        public List<Claim> Claims { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public Credential Credential { get; set; }
        public async Task OnGetAsync()
        {
            if (Request.Cookies["Username"] != null)
            {
                Response.Redirect("Home");

            }
            Schemes = await GetExternalProvidersAsync(HttpContext);
        }

        public string OnPostForgotPassword(string submit, string Username, string Password, string Femail)
        {
            if (submit == "Request")
            {
                var Cstring2 = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
                var client2 = new MongoClient(Cstring2);


                var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
                var database2 = client2.GetDatabase(dbName);
                var collection2 = database2.GetCollection<ForgotDetails>("UsersLogin");
                var document = new ForgotDetails();
                document.EmailID = Femail;
                document.Type = "Fail";
                collection2.InsertOne(document);
                return "Success";
            }
            else return "Failed";
        }
        public void OnPostLoginForgotPassword(string submit, string UNAMEHidden)
        {
            ViewData["Disp"] = "E";
        }

        public LoginDetails IsUserAuthorized(string submit, string Username, string Password, string Femail)
        {
            var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
            var client = new MongoClient(Cstring);

            var dbList = client.ListDatabases().ToList();

            var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
            var database = client.GetDatabase(dbName);

            var collection = database.GetCollection<LoginDetails>("LoginDetails");
            var aggResult = collection.Aggregate().ToList();
            
            var fuser = aggResult.Find(x => x.UserName == Username);
           
            if (fuser != null)
            {
                if (fuser.Password == Password)
                {                   
                    return fuser;
                }
                else return null;
            }
            else return null;

        }
        public async Task OnPostLoginAsync(string submit, string Username, string Password, string Femail)
        {
            if (submit == "Log In")
            {
                var fuser = IsUserAuthorized(submit, Username, Password, Femail);
                if (fuser != null)
                {
                    var claims = new List<Claim>{
            new Claim(ClaimTypes.Name, fuser.UserName),
            new Claim("FirstName", fuser.FirstName),
            new Claim("LastName", fuser.LastName) };
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {

                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    var results = User.Claims;
                    Response.Redirect("Home");
                }
                else
                {
                    ViewData["Lerror"] = "T";
                }
            }
            if (submit == "Forgot Password")
            {
                ViewData["Disp"] = "E";
            }
        }



public async Task<IActionResult> OnPostLoginExternal([FromForm] string provider)
{
    if (string.IsNullOrWhiteSpace(provider))
    {
        return BadRequest();
    }

    if (await IsProviderSupportedAsync(HttpContext, provider))
    {
        var chalresult = Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/"
        }, provider);
        Claims = User.Claims.ToList();
        if (User.AccessToken() is { } accessToken)
        {
            var client = new GitHubClient(new ProductHeaderValue("test"))
            {
                Credentials = new Credentials(accessToken)
            };
            GitHubUser = await client.User.Get(User.Identity?.Name);
        }
        return chalresult;
    }
    else
    {
        return BadRequest();
    }

}

private static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(HttpContext context)
{
    var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
    return (await schemes.GetAllSchemesAsync())
        .Where(scheme => !string.IsNullOrEmpty(scheme.DisplayName))
        .ToArray();
}

private static async Task<bool> IsProviderSupportedAsync(HttpContext context, string provider) =>
    (await GetExternalProvidersAsync(context))
    .Any(scheme => string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase));

    }

    internal class ForgotDetails
{
    [BsonElement("Type")]
    public string Type { get; set; }
    [BsonElement("EmailID")]
    public string EmailID { get; set; }
}

public class LoginDetails
{

    [BsonElement("_id")]
    //[BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    [BsonElement("UserName")]
    public string UserName { get; set; }
    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonElement("FirstName")]
    public string FirstName { get; set; }

    [BsonElement("LastName")]
    public string LastName { get; set; }

    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("DateofBirth")]
    public string DateofBirth { get; set; }

    [BsonElement("Gender")]
    public string Gender { get; set; }
}

public class Credential
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }





}
}
