using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Project1.Pages
{
    public class HomeModel : PageModel
    {
        [BindProperty]
        public string BillType { get; set; }
        [BindProperty]
        public string DueDate { get; set; }

        [BindProperty]
        public string DueAmount { get; set; }

        [BindProperty]
        public List<BillDetail> billDetails { get; set; }

        [BindProperty]
        public List<BillDetail> PaidbillDetails
        {
            get; set;
        }
        [BindProperty]
        public string PaidCount { get; set; }

        [BindProperty]
        public string UnpaidCount { get; set; }
        public void OnPost(string pay)
        {

            billDetails.Clear();
            var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
            var client = new MongoClient(Cstring);

            var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
            var database = client.GetDatabase(dbName);

            var collection = database.GetCollection<BillDetail>("BillDetail");

            var filter = Builders<BillDetail>.Filter.Eq("BillNumber", pay);
            var update = Builders<BillDetail>.Update.Set("Status", "Paid");

            collection.UpdateOne(filter, update);



            SetData();
        }

        public void OnGet()
        {
            try
            {
                if (Request.Cookies["Username"] != null)
                {
                    string? var = Request.Cookies["Username"].ToString();
                    ViewData["Username"] = var;
                    string varfname = Request.Cookies["FirstName"].ToString();
                    string varLname = Request.Cookies["LastName"].ToString();
                    ViewData["FullName"] = varfname + ' ' + varLname;
                    SetData();
                }
                else {
                    Response.Redirect("Login");
                }

            }
            catch {
                Response.Redirect("Login");
            }

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
                var aggResult = collection.Aggregate().ToList();
                string var = Request.Cookies["Username"].ToString();
                UnpaidCount = aggResult.ToList().Where(x => x.UserName == var).ToList().Where(x => x.Status == "Unpaid").ToList().Count.ToString();
                PaidCount = aggResult.ToList().Where(x => x.UserName == var).ToList().Where(x => x.Status == "Paid").ToList().Count.ToString();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                {
                    Response.Redirect("Error");
                }
            }

        }
        public class BillDetail
        {
            [BsonElement("_id")]
            //[BsonRepresentation(BsonType.ObjectId)]
            public ObjectId Id { get; set; }

            [BsonElement("BillNumber")]
            public string BillNumber { get; set; }

            [BsonElement("BillType")]
            public string BillType { get; set; }

            [BsonElement("Amount")]
            public int Amount { get; set; }

            [BsonElement("DueDate")]
            public string DueDate { get; set; }

            [BsonElement("Status")]
            public string Status { get; set; }

            [BsonElement("IssueDate")]
            public string IssueDate { get; set; }

            [BsonElement("UserName")]
            public string UserName { get; set; }

        }
    }
}
