using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using Project1.Helpers;
using System.Security.Claims;

namespace Project1.Pages
{
    [Authorize]
    public class AllBillModel : PageModel
    {
        [BindProperty]
        public List<BillDetail> PaidbillDetails
        {
            get; set;
        }

        public void OnGet()
        {
            try
            {
                var results = User.Claims;
                string var = results.FirstClaim(ClaimTypes.Name); //ClaimTypes.Name  Request.Cookies["Username"].ToString();
                ViewData["Username"] = results.FirstClaim(ClaimTypes.Name);
                string varfname = results.FirstClaim("FirstName");
                string varLname = results.FirstClaim("LastName");
                ViewData["FullName"] = varfname == null ? var : varfname + ' ' + varLname;
                SetData();

               

            }
            catch { }
        }

        private void SetData()
        {
            try
            {
                var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
                var client = new MongoClient(Cstring);

                var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
                var database = client.GetDatabase(dbName);

                var collection = database.GetCollection<BillDetail>("BillDetail");
                var results = User.Claims;
                string var = results.FirstClaim(ClaimTypes.Name);
               
                var aggResult = collection.Aggregate().ToList().Where(x => x.UserName == var).ToList();
                
                PaidbillDetails = aggResult.ToList();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                {
                    Response.Redirect("Error");
                }
            }

        }
    }
}
