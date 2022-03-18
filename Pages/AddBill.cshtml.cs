using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Project1.Helpers;
using System.Security.Claims;

namespace Project1.Pages
{
    [Authorize]
    public class AddBillModel : PageModel
    {
        [BindProperty]
        public List<BillType> BillType { get; set; }

        [BindProperty]
        public List<BillDetail> BillDetail { get; set; }
        public void OnGet()
        {
           
            try
            {
                var results = User.Claims;
                string var = results.FirstClaim(ClaimTypes.Name); //ClaimTypes.Name  Request.Cookies["Username"].ToString();
                ViewData["Username"] = results.FirstClaim(ClaimTypes.Name);
                string varfname = results.FirstClaim("FirstName");
                string varLname = results.FirstClaim("LastName");
                ViewData["FullName"] = varfname==null? var : varfname + ' ' + varLname;
                SetData();



            }
            catch { }
        }
        public void OnPost(string submit, string BillTy, string Status, int Amount, string Month, string Year, string Day, string BillTyTxt)
        {
            try
            {
                var Cstring2 = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
                var client2 = new MongoClient(Cstring2);


                var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
                var database2 = client2.GetDatabase(dbName);
                var collection2 = database2.GetCollection<BillDetail>("BillDetail");
                var document = new BillDetail();
                document.BillNumber = BillTy + DateTime.Now.ToShortDateString();
                if (BillTy == "Other")
                {
                    BillTy = BillTyTxt;
                }
                document.BillType = BillTy;
                document.Amount = Amount;
                document.Status = Status;
                //document.DueDate = Year + '-' + Month + '-' + Day;

                document.DueDate = Month + '/' + Day + '/' + Year;
                var results = User.Claims;
                string var = results.FirstClaim(ClaimTypes.Name); 
                document.UserName = var;
                collection2.InsertOne(document);
                ViewData["DataStatus"] = "S";
            }
            catch
            {
                ViewData["DataStatus"] = "F";
            }
        }

        private void SetData()
        {
            var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
            var client = new MongoClient(Cstring);

            var dbList = client.ListDatabases().ToList();

            var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<BillType>("BillDetail");

            var collection2 = database.GetCollection<BillType>("BillType");
            var aggResult = collection2.Aggregate().ToList();
            BillType = aggResult.ToList(); ;

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

    public class BillType
    {
        [BsonElement("_id")]
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }
    }
}
