using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;

namespace Project1.Pages
{
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
            string var2 = Request.Cookies["Username"].ToString();
            var filter  = Builders<BillDetail>.Filter.Eq("BillNumber", pay); 
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
                string var = Request.Cookies["Username"].ToString();
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
