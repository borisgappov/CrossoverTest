using LogApi.Models;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace LogApi.App_Code
{
    /// <summary>
    /// Everywhere necessary tools 
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Rate limit exceeded timeout
        /// </summary>
        public static int RateLimitExceedTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["RateLimitExceedTimeout"]);

        /// <summary>
        /// Maximum count of requests per minute
        /// </summary>
        public static int RequestsPerMinuteCount = Convert.ToInt32(ConfigurationManager.AppSettings["RequestsPerMinuteCount"]);

        /// <summary>
        /// Session life time - time interval user inactivity after which the information about the authenticated user will be removed 
        /// </summary>
        public static int SessionLifeTime = Convert.ToInt32(ConfigurationManager.AppSettings["SessionLifeTime"]);

        /// <summary>
        /// The collection of authenticated users
        /// </summary>
        public static ConcurrentDictionary<string, UserData> Users
        {
            get
            {
                return (ConcurrentDictionary<string, UserData>)HttpContext.Current?.Application["users"];
            }
            set
            {
                HttpContext.Current?.Application.Add("users", value);
            }
        }

        /// <summary>
        /// Returns true when input string is base64 encoded
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>Encoded or not</returns>
        public static bool IsBase64(string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,2}$");
        }

        /// <summary>
        /// Returns UserData object when user is authenticated
        /// </summary>
        /// <param name="UserName">User name</param>
        /// <returns>UserData object</returns>
        public static UserData GetUserData(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return null;
            }
            UserData user = null;
            Users.TryGetValue(UserName, out user);
            return user;
        }

        /// <summary>
        /// Returns stringifyed guid without '-'
        /// </summary>
        /// <returns></returns>
        public static string NewGuidString()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

    }
}