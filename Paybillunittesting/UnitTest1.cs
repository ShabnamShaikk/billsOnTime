using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project1.Pages;
using Project1.Pages.Account;

namespace PayBillsTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoginWithValidCredentials()
        {
            LoginModel obj = new LoginModel();
            var result = obj.IsUserAuthorized("Log In", "shabnam", "sshaik", "shabnam@gmail.com");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginWithInvalidCredentials()
        {
            LoginModel obj = new LoginModel();
            var result = obj.IsUserAuthorized("test", "123", "test", "123");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ForgotPasswordWithValidCredentials()
        {
            LoginModel obj = new LoginModel();
            var result = obj.OnPostForgotPassword("Request", "username", "password", "user1@demo.com");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ForgotPasswordWithInvalidCredentials()
        {
            LoginModel obj = new LoginModel();
            var result = obj.OnPostForgotPassword("Request1", "username", "password", "user1@demo.com");
            Assert.IsNull(result);
        }
    }
}