using EncryptStringSample;
using System;
using System.Linq;
using System.Transactions;

namespace LogApi.App_Code
{
    /// <summary>
    /// Database access class
    /// </summary>
    public class DataLayer
    {
        /// <summary>
        /// Returns application object that was found by the ID and secret
        /// </summary>
        /// <param name="application_id">Application id</param>
        /// <param name="application_secret">Secret</param>
        /// <returns>Application object</returns>
        public application GetAppByNameAndSecret(string application_id, string application_secret) {
            application app = null;
            using (var ctx = new CrossoverDataContext())
            {
                app = ctx.applications
                    .Where(x => x.application_id == application_id).ToArray()
                    .Where(x => StringCipher.Decrypt(x.secret) == application_secret)
                    .FirstOrDefault();
            }
            return app;
        }

        /// <summary>
        /// Adds record to log table
        /// </summary>
        /// <param name="record">Record object</param>
        /// <returns>Id of saved record</returns>
        public int AddLog(log record) {
            int log_id = 0;
            using (var ctx = new CrossoverDataContext())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    ctx.logs.InsertOnSubmit(record);
                    ctx.SubmitChanges();
                    log_id = record.log_id;
                    scope.Complete();
                }
            }
            return log_id;
        }

        /// <summary>
        /// Adds record to application table
        /// </summary>
        /// <param name="app">Application object</param>
        /// <returns>Same application object</returns>
        public application AddApplication(application app) {
            String secret = app.secret;
            app.secret = StringCipher.Encrypt(secret);
            using (var ctx = new CrossoverDataContext())
            {
                using (var scope = new TransactionScope( TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    ctx.applications.InsertOnSubmit(app);
                    ctx.SubmitChanges();
                    scope.Complete();
                }
            }
            app.secret = secret;
            return app;
        }

    }
}