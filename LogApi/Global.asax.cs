using System;
using System.Linq;
using System.Diagnostics;
using System.Web.Http;
using LogApi.App_Start;
using System.Collections.Concurrent;
using LogApi.Models;
using static LogApi.App_Code.Utils;
using System.Web;
using System.ComponentModel;

namespace LogApi
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // Web API configuration
            GlobalConfiguration.Configure(WebApiConfig.Configure);

            // Creating of users collection
            Users = new ConcurrentDictionary<string, UserData>();

            // Adding users collection to cache for access by background worker
            HttpRuntime.Cache.Add("Users", Users, null, DateTime.Now.AddYears(10), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);

            // Background worker for check the expiration of the lifetime of the user session
            BackgroundWorker worker = new BackgroundWorker();
            Application["UsersWorker"] = worker;
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            worker.RunWorkerAsync();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Delete worker
            if (Application["UsersWorker"] != null)
            {
                BackgroundWorker worker = (BackgroundWorker)Application["UsersWorker"];
                if (worker != null)
                    worker.CancelAsync();
            }
        }

        /// <summary>
        /// Background worker job
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            var users = (ConcurrentDictionary<string, UserData>)HttpRuntime.Cache.Get("Users");
            var removeKeys = users.Values
                .Where(x => {
                    return (DateTime.Now - x.RequestTicks.GetLast()).TotalMinutes > SessionLifeTime;
                })
                .Select(x => x?.Principal?.Identity?.Name)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            UserData data;
            if(removeKeys != null)
            {
                foreach(var key in removeKeys)
                {
                    users.TryRemove(key, out data);
                }
            }
        }

        private static void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                // worker started once per minute
                System.Threading.Thread.Sleep(60000);
                worker.RunWorkerAsync();
            }
        }


    }
}