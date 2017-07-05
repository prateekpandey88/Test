using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web;

namespace SignixDemo.Helpers
{
    public struct SignixEvents
    {
        /// <summary>
        /// Cause: Transaction activated / started.
        /// </summary>
        public const string Send = "send";

        /// <summary>
        /// Cause: Party completed all their required actions.
        /// </summary>
        public const string PartyComplete = "partycomplete";

        /// <summary>
        /// Cause: Transaction completed.
        /// </summary>
        public const string Complete = "complete";

        /// <summary>
        /// Cause: Transaction suspended.
        /// </summary>
        public const string Suspend = "suspend";

        /// <summary>
        /// Cause: Transaction cancelled.
        /// </summary>
        public const string Cancel = "cancel";

        /// <summary>
        /// Cause: Transaction expired.
        /// </summary>
        public const string Expire = "expire";
    }

    public class Common
    {
        public static string ServerUploadFolderPath => HttpContext.Current.Server.MapPath("~/Logs");

        public static void WriteLog(string str)
        {
            File.AppendAllText(ServerUploadFolderPath + "\\error.txt", $"{DateTime.Now}: {str} {Environment.NewLine}");
        }

        public static void InitializeMembership()
        {
            try
            {
                Database.SetInitializer<DataLayer.DataContext>(null);

                using (var context = new DataLayer.DataContext())
                {
                    if (!context.Database.Exists())
                    {
                        // Create the SimpleMembership database without Entity Framework migration schema
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Membership database could not be initialized.", ex);
            }
        }
    }
}