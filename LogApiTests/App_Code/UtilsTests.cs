using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Web;
using System.Collections.Concurrent;
using LogApi.Models;
using Http.TestLibrary;

namespace LogApi.App_Code.Tests
{
    [TestClass()]
    public class UtilsTests
    {
        [TestMethod()]
        public void IsBase64Test()
        {
            Assert.IsTrue(Utils.IsBase64(Convert.ToBase64String(Encoding.UTF8.GetBytes("test string"))));
        }

        [TestMethod]
        public void GetUserDataTest()
        {
            UserData userData = null;
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost:8080"));
                HttpContext.Current.ApplicationInstance = new HttpApplication();
                Utils.Users = new ConcurrentDictionary<string, UserData>();
                Utils.Users.TryAdd("application_id", new UserData());
                userData = Utils.GetUserData("application_id");
            }
            Assert.IsNotNull(userData);
        }

        [TestMethod()]
        public void NewGuidStringTest()
        {
            Guid guid;
            Assert.IsTrue(Guid.TryParse(Utils.NewGuidString(), out guid));
        }
    }
}