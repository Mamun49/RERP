using RERP.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class HomeController : Controller
    {
        CategoryController cate = new CategoryController();
        ColorController color = new ColorController();
        SizeController size = new SizeController();
        ExpenceTypeController type = new ExpenceTypeController();
        public ActionResult Index()
        {
            IEnumerable<DropDownModel> catedata = cate.GetCategoryList() as IEnumerable<DropDownModel>;
            IEnumerable<DropDownModel> colordata = color.GetColorList();
            IEnumerable<DropDownModel> sizedata = size.GetSizeList();
            ViewBag.CategoryLists = catedata;
            ViewBag.ColorLists = colordata;
            ViewBag.SizeLists = sizedata;
            IEnumerable<DropDownModel> exptype = type.GetTypeList() as IEnumerable<DropDownModel>;
            ViewBag.TypeLists = exptype;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult KeepAlive()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            { // Return any response; it could be a simple string or a JSON object
                return Content("Session is alive");

            }
            else { return RedirectToRoute("login"); }
        }
    }
}