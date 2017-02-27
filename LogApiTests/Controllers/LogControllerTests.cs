using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using LogApi.App_Code;
using System.Web.Http.Results;
using LogApi.Models;

namespace LogApi.Controllers.Tests
{
    [TestClass()]
    public class LogControllerTests
    {
        [TestMethod]
        public void IndexTest()
        {
            Assert.IsTrue(Task.Run(async () =>
            {
                using (var ctx = new CrossoverDataContext())
                {
                    var app = ctx.applications.FirstOrDefault();
                    string logger = "LogApi.App_Code.Tests.LogControllerTests_" + new Random().Next(100, 500).ToString();

                    var result = await new LogController().Index(new log
                    {
                        application_id = app.application_id,
                        level = "Info",
                        logger = logger,
                        message = "Test of LogController"
                    }) as OkNegotiatedContentResult<AddLogResult>;

                    var savedLogItem = ctx.logs.FirstOrDefault(x => x.logger == logger);
                    var saved = savedLogItem != null;

                    ctx.logs.DeleteOnSubmit(savedLogItem);
                    ctx.SubmitChanges();

                    return result.Content.success && saved;
                }
            }).GetAwaiter().GetResult());
        }
    }
}