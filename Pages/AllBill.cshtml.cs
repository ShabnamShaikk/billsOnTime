using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace Project1.Pages
{
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
                if (Request.Cookies["Username"] != null)
                {
                    string var = Request.Cookies["Username"].ToString();
                    ViewData["Username"] = var;
                    string varfname = Request.Cookies["FirstName"].ToString();
                    string varLname = Request.Cookies["LastName"].ToString();
                    ViewData["FullName"] = varfname + ' ' + varLname;
                    SetData();

                }
                else
                {
                    Response.Redirect("Login");
                }

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
                string var = Request.Cookies["Username"].ToString();
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
