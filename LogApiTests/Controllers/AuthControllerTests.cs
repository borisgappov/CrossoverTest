using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web;
using LogApi.App_Code;
using LogApi.Models;
using System.Collections.Concurrent;
using Http.TestLibrary;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Results;

namespace LogApi.Controllers.Tests
{
    [TestClass()]
    public class AuthControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            var UserName = Utils.NewGuidString();
            string Token = Utils.NewGuidString();
            string ResultToken = string.Empty;
            using (HttpSimulator simulator = new HttpSimulator())
            {
                simulator.SimulateRequest(new Uri("http://localhost:8080"));
                HttpContext.Current.ApplicationInstance = new HttpApplication();
                Utils.Users = new ConcurrentDictionary<string, UserData>();
                Utils.Users.TryAdd(UserName, new UserData { Token = Token });
                var principal = new GenericPrincipal(new GenericIdentity(UserName), null);
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
                var result = new AuthController().Index() as OkNegotiatedContentResult<AuthSuccessResult>;
                ResultToken = result.Content.access_token;
            }
            Assert.IsTrue(ResultToken == Token);
        }
    }
}