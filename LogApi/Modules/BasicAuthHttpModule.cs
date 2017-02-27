using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using LogApi.App_Code;
using static LogApi.App_Code.Utils;
using LogApi.Models;

namespace LogApi.Modules
{
    /// <summary>
    /// Basic authentication module
    /// The source code was written using this example https://www.asp.net/web-api/overview/security/basic-authentication
    /// </summary>
    public class BasicAuthHttpModule : IHttpModule
    {
        /// <summary>
        /// Data access object
        /// </summary>
        private static DataLayer data = new DataLayer();

        /// <summary>
        /// Basic authentication realm
        /// </summary>
        private const string Realm = "LogApi";

        /// <summary>
        /// Initialisation of module method
        /// </summary>
        /// <param name="context">Http context object</param>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        /// <summary>
        /// Creates user principal
        /// </summary>
        /// <param name="UserName">User name</param>
        private static void SetPrincipal(string UserName)
        {
            if (HttpContext.Current != null)
            {
                IPrincipal principal = null;
                UserData user = null;
                Users.TryGetValue(UserName, out user);
                if (user == null)
                {
                    principal = new GenericPrincipal(new GenericIdentity(UserName), null);
                    user = new UserData
                    {
                        Principal = principal,
                        Token = NewGuidString()
                    };
                    Users.TryAdd(UserName, user);
                }
                else
                {
                    principal = user.Principal;
                }

                DateTime? first = user.RequestTicks.Enqueue(DateTime.Now);
                if (first != null && first != DateTime.MinValue && (first.Value - DateTime.Now).TotalSeconds < 60)
                {
                    user.BlockedTo = DateTime.Now.AddSeconds(RateLimitExceedTimeout);
                }

                if (user.BlockedTo == null)
                {
                    Thread.CurrentPrincipal = principal;
                    HttpContext.Current.User = principal;
                }
                else
                {
                    HttpContext.Current.Response.StatusCode = 403;
                    HttpContext.Current.Response.Status = "403 Application Rate Limit Exceeded";
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
        }

        /// <summary>
        /// Checks for the existence of the application by ID and secret
        /// </summary>
        /// <param name="application_id">Application id</param>
        /// <param name="application_secret">Secret</param>
        /// <returns></returns>
        private static bool CheckApplication(string application_id, string application_secret)
        {
            return data.GetAppByNameAndSecret(application_id, application_secret) != null;
        }

        /// <summary>
        /// Provides token authentication
        /// </summary>
        /// <param name="Token">Token</param>
        /// <param name="application_id">Id of found application</param>
        /// <returns></returns>
        private static bool CheckApplication(string Token, out string application_id)
        {
            var userData = Users.Values.ToArray().Where(x => x.Token == Token).FirstOrDefault();
            application_id = userData?.Principal?.Identity?.Name;
            return !string.IsNullOrEmpty(application_id);
        }

        /// <summary>
        /// Authenticates user by basic auth credentials, application id and secret, token
        /// </summary>
        /// <param name="credentials">Credentials - base64 string or application id and secret pair, or token</param>
        private static void AuthenticateUser(string credentials)
        {
            try
            {
                Guid guid = Guid.Empty;
                bool verified = false;
                string application_id = string.Empty;
                if (Guid.TryParse(credentials, out guid))
                {
                    verified = CheckApplication(credentials, out application_id);
                }
                else
                {
                    credentials = IsBase64(credentials) ?
                        Encoding.UTF8.GetString(Convert.FromBase64String(credentials)) :
                        credentials;

                    int separator = credentials.IndexOf(':');
                    application_id = credentials.Substring(0, separator);
                    verified = CheckApplication(application_id, credentials.Substring(separator + 1));
                }

                if (verified)
                {
                    SetPrincipal(application_id);
                }
                else
                {
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        /// <summary>
        /// Application authenticate request event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = authHeader.ToLower().StartsWith("basic") ?
                        AuthenticationHeaderValue.Parse(authHeader) :   // base64 (standard)    
                    new AuthenticationHeaderValue("basic", authHeader); // like in task deskription


                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        /// <summary>
        /// Application end request event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        /// <summary>
        /// Dispose method implemented from IHttpModule interface
        /// </summary>
        public void Dispose()
        {
        }
    }
}