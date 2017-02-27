using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LogApi.App_Code.Tests
{
    [TestClass()]
    public class DataLayerTests
    {
        [TestMethod()]
        public void GetAppByNameAndSecretTest()
        {
            AddApplicationTest();
        }

        [TestMethod()]
        public void AddLogTest()
        {
            DataLayer dl = new DataLayer();
            string app_id = "test app " + new Random().Next(100, 500).ToString();
            var app = dl.AddApplication(new application
            {
                application_id = app_id,
                display_name = "Test application",
                secret = Guid.NewGuid().ToString().Replace("-", string.Empty)
            });
            var id = dl.AddLog(new log
            {
                application_id = app.application_id,
                level = "Info",
                logger = "LogApi.App_Code.Tests.AddLogTest",
                message = "Test of database layer"
            });
            using (var ctx = new CrossoverDataContext())
            {
                var logItem = ctx.logs.FirstOrDefault(x => x.log_id == id);
                Assert.IsTrue(logItem.log_id == id);
                ctx.logs.DeleteOnSubmit(logItem);
                ctx.applications.DeleteOnSubmit(ctx.applications.FirstOrDefault(x => x.application_id == app_id));
                ctx.SubmitChanges();
            }
        }

        [TestMethod()]
        public void AddApplicationTest()
        {
            DataLayer dl = new DataLayer();
            string app_id = "test app " + new Random().Next(100, 500).ToString();
            var app = dl.AddApplication(new application
            {
                application_id = app_id,
                display_name = "Test application",
                secret = Guid.NewGuid().ToString().Replace("-", string.Empty)
            });
            var dbApp = dl.GetAppByNameAndSecret(app.application_id, app.secret);
            Assert.IsTrue(dbApp.application_id == app_id);
            using (var ctx = new CrossoverDataContext())
            {
                ctx.applications.DeleteOnSubmit(ctx.applications.FirstOrDefault(x => x.application_id == app_id));
                ctx.SubmitChanges();
            }
        }
    }
}