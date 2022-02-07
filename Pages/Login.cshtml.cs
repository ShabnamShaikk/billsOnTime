using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Attributes;

namespace Project1.Pages.Account
{
    public class LoginModel : PageModel
    {
        private IMongoCollection<LoginDetails> _UserLogin;

        public Credential Credential { get; set; }
        public void OnGet()
        {
            if (Request.Cookies["Username"] != null)
            {
                Response.Redirect("Home");

            }
        }

        public void OnPost(string submit, string Username, string Password, string Femail)
        {
            if (submit == "Log In")
            {

                //                var client = new MongoClient(
                //    "mongodb+srv://<username>:<password>@<cluster-address>/test?w=majority"
                //);
                //                var database = client.GetDatabase("test");

                var Cstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
                var client = new MongoClient(Cstring);

                var dbList = client.ListDatabases().ToList();

                var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
                var database = client.GetDatabase(dbName);



                //MongoCollection collection = (MongoCollection)database.GetCollection<LoginDetails>("LoginDetails");
                //var query = collection.AsQueryable()
                // .Where(p => p.Age > 21)
                //.Select(p => new { p.UserName, p.Password });
                var collection = database.GetCollection<LoginDetails>("LoginDetails");
                var aggResult =  collection.Aggregate().ToList();
                //MongoCursor<LoginDetails> cursor = database.FindAllAs<LoginDetails>();
                // cursor.SetLimit(5);
                var fuser = aggResult.Find(x => x.UserName == Username);
                // var list = cursor.ToList();
                if (fuser != null)
                {
                    if (fuser.Password == Password)
                    {
                        Response.Cookies.Append("Username", fuser.UserName);

                        Response.Cookies.Append("FirstName", fuser.FirstName);
                        Response.Cookies.Append("LastName", fuser.LastName);
                        Response.Redirect("Home");
                    }
                    else {
                        ViewData["Lerror"] = "T";
                    }

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
            }
        }
    }

    internal class ForgotDetails
    {
        [BsonElement("Type")]
        public string Type { get; set; }
        [BsonElement("EmailID")]
        public string EmailID { get; set; }
    }

    internal class LoginDetails
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
