using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Project1.Pages
{
    public class SignUpModel : PageModel
    {
        public void OnGet()
        {
        }
        public void OnPost(string FirstName, string LastName, string EmailID, string Month, string Year, string Day, string Password, string radios)
        {
            try
            {
                var Cstring2 = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["ConnectionString"];
                var client2 = new MongoClient(Cstring2);


                var dbName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DatabaseName"];
                var database2 = client2.GetDatabase(dbName);
                var collection2 = database2.GetCollection<SignUpDetail>("LoginDetails");
                var aggResult = collection2.Aggregate().ToList();
                string Userid = EmailID;
                var find = aggResult.ToList().Where(x => x.UserName == Userid).ToList().Count;
                if (find == 0)
                {
                    var document = new SignUpDetail();
                    document.UserName = EmailID;
                    document.FirstName = FirstName;
                    document.LastName = LastName;
                    document.Email = EmailID;
                    document.Gender = radios;
                    document.Password = Password;
                    document.DateofBirth = Year + '-' + Month + '-' + Day;
                    collection2.InsertOne(document);
                    ViewData["DataStatus"] = "S";
                }
                else
                {
                    ViewData["DataStatus"] = "F";
                    ViewData["EmStat"] = "E";
                }
            }
            catch
            {
                ViewData["DataStatus"] = "F";
            }
        }

        
    }

    internal class SignUpDetail
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
}
