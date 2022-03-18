using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using Project1.Helpers;
using System.Security.Claims;

namespace Project1.Pages
{
    [Authorize]
    public class PaybillModel : PageModel
    {

        [BindProperty]
        public List<BillDetail> billDetails { get; set; }
        public void OnPost(string pay)
        {

            billDetails.Clear();
            var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
            var client = new MongoClient(Cstring);

            var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
            var database = client.GetDatabase(dbName);

            var collection = database.GetCollection<BillDetail>("BillDetail");
            var results = User.Claims;
            string var2 = results.FirstClaim(ClaimTypes.Name); //ClaimTypes.Name  Request.Cookies["Username"].ToString();
            ViewData["Username"] = results.FirstClaim(ClaimTypes.Name);
            string varfname = results.FirstClaim("FirstName");
            string varLname = results.FirstClaim("LastName");
            ViewData["FullName"] = varfname == null ? var2 : varfname + ' ' + varLname;

            var filter = Builders<BillDetail>.Filter.Eq("BillNumber", pay);
            var filter2 = Builders<BillDetail>.Filter.Eq("UserName", var2);
            var update = Builders<BillDetail>.Update.Set("Status", "Paid");
            var andFilter = filter & filter2;
            collection.UpdateOne(andFilter, update);



            SetData();
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
                var results = User.Claims;
                string var = results.FirstClaim(ClaimTypes.Name);
                var collection = database.GetCollection<BillDetail>("BillDetail");
                var aggResult = collection.Aggregate().ToList();
                billDetails = aggResult.ToList().Where(x => x.UserName == var).ToList().Where(x => x.Status == "Unpaid").ToList();



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
