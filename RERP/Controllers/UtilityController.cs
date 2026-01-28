using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class UtilityController : Controller
    {
        // GET: Utility
        public ActionResult Index()
        {
            return View();
        }
        public static string GetServerName()
        {
            return System.Environment.MachineName;
        }

        public static string GetClientIP(HttpRequestBase request)
        {
            string clientIP = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(clientIP))
            {
                clientIP = request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                // If HTTP_X_FORWARDED_FOR contains multiple IP addresses, extract the first one (IPv4)
                clientIP = clientIP.Split(',').FirstOrDefault()?.Trim();
            }

            // If the extracted IP is still null or empty, use the remote address
            if (string.IsNullOrEmpty(clientIP))
            {
                clientIP = request.ServerVariables["REMOTE_ADDR"];
            }

            return clientIP;
        }
    }
}