using System;
using System.Web;
using System.Web.Http;
using SignixDemo.AppStart;
using SignixDemo.DataLayer;
using SignixDemo.Helpers;
using SignixDemo.Models.Entities;

namespace SignixDemo
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Common.InitializeMembership();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var url = HttpContext.Current.Request.RawUrl;

            Common.WriteLog($"URL: {url}");

            if (url.ToLower().Contains("notificationlistener"))
            {
                Repository.Instance.Insert(new NotificationUrls
                {
                    Url = url,
                    CreatedOn = DateTime.Now
                });
                Repository.Dispose();
                Response.Redirect($"https://dev-app.ncontracts.com/{url.Replace("/SignixDemo/", "")}");
            }


        }
    }
}