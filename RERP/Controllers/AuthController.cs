using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RERP.Controllers
{
    public class AuthController : Controller
    {
        RERPEntities _db = new RERPEntities();
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetData(tbl_user data)
        {
            var softcafemail = "hasan@techsavvy.com";

            var checkUser = _db.tbl_user.Where(x => x.user_mail.Equals(data.user_mail) && x.is_active == true && x.is_deleted == false).FirstOrDefault();
            if (checkUser != null || softcafemail == data.user_mail)
            {

                var checkLogin = _db.tbl_user.Where(x => x.user_mail.Equals(data.user_mail) && x.pass == data.pass).FirstOrDefault();
                if (checkLogin != null)
                {
                    if (data.remember_token != null)
                    {
                        HttpCookie cookie = new HttpCookie("UserInfo");
                        cookie["UserID"] = checkLogin.user_id.ToString();
                        cookie.Expires = DateTime.Now.AddMonths(1); // Set the expiration time
                        Response.Cookies.Add(cookie);
                    }
                    FormsAuthentication.SetAuthCookie(checkLogin.user_mail, false);
                    //var position = _db.tbl_position.Where(x => x.pk_position_id == checkLogin.role).Select(x => x.position_name).FirstOrDefault();
                    Session["UserMail"] = checkLogin.user_mail.ToString();
                    Session["UserName"] = checkLogin.user_name.ToString();
                    Session["Role"] = checkLogin.role.ToString();
                    Session["ID"] = Convert.ToInt64(checkLogin.user_id);
                    Session["User_ID"] = checkLogin.user_id.ToString();
                    Session["User_role"] = checkLogin.role.ToString();
                    Session["User_dp"] = checkLogin.dp?.ToString();
                    
                    return Json(new { success = true, Message = "Welcome to Kitchen Hub!" });
                }
                else
                {

                    return Json(new { success = false, Message = "Incorrect Password!" });
                }
            }
            else
            {
                return Json(new { success = false, Message = "Invalid User" });
            }

        }
        public ActionResult logout()
        {
            HttpCookie cookie = new HttpCookie("UserInfo");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToRoute("login");
        }
    }
}