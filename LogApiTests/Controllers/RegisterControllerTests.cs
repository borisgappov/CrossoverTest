using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using LogApi.App_Code;
using LogApi.Models;
using System.Web.Http.Results;

namespace LogApi.Controllers.Tests
{
    [TestClass()]
    public class RegisterControllerTests
    {
        [TestMethod()]
        public void IndexTest()
        {
            Assert.IsTrue(Task.Run(async () =>
            {
                using (var ctx = new CrossoverDataContext())
                {
                    var app = ctx.applications.FirstOrDefault();
                    string app_name = "test_app_" + new Random().Next(100, 500).ToString();

                    var result = await new RegisterController().Index(new AppRegistration
                    {
                        display_name = app_name
                    }) as OkNegotiatedContentResult<RegisterAppResult>;

                    var savedApp = ctx.applications.FirstOrDefault(x => x.display_name == app_name);
                    var saved = savedApp != null;

                    ctx.applications.DeleteOnSubmit(savedApp);
                    ctx.SubmitChanges();

                    return saved;
                }
            }).GetAwaiter().GetResult());
        }
    }
}