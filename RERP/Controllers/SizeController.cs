using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class SizeController : Controller
    {
        // GET: Size
        RERPEntities _db = new RERPEntities();
        // GET: Category
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<tbl_size> itemlist = (from ip in _db.tbl_size
                                               where ip.is_deleted == false
                                               select ip).ToList();


                ViewBag.getsize = itemlist;
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpGet]
        public ActionResult GetData(int id)
        {
            tbl_size result = _db.tbl_size.Where(x => x.size_id == id).FirstOrDefault();

            var data = new
            {
                size_id = result.size_id,
                size_name = result.size_name,
                is_active = result.is_active
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(tbl_size data)
        {

            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                tbl_size _pro = new tbl_size();
                _pro.size_name = data.size_name;
                _pro.is_active = data.is_active;
                _pro.is_deleted = false;
                _pro.created_by = user_id;
                _pro.created_pc = UtilityController.GetServerName();
                _pro.created_ip = UtilityController.GetClientIP(Request);
                _pro.created_at = DateTime.Now;
                _db.tbl_size.Add(_pro);
                _db.SaveChanges();

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        [HttpPost]
        public ActionResult UpdateCategory(tbl_size data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_size.Where(x => x.size_id == data.size_id).FirstOrDefault();
                _pro.size_name = data.size_name;
                _pro.is_active = data.is_active;
                _pro.updated_by = user_id;
                _pro.updated_pc = UtilityController.GetServerName();
                _pro.updated_at = DateTime.Now;
                _pro.updated_ip = UtilityController.GetClientIP(Request);
                _db.SaveChanges();
                return Json(new { success = true, Message = "Update Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _client = _db.tbl_size.Where(x => x.size_id == id).FirstOrDefault();
            _client.is_deleted = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }
        public List<DropDownModel> GetSizeList()
        {
            List<DropDownModel> data = (from cate in _db.tbl_size
                                        where cate.is_deleted == false
                                        select new DropDownModel
                                        {
                                            id = cate.size_id,
                                            dd_value = cate.size_name
                                        }).ToList();
            return data;

        }
    }
}