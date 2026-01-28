using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class CategoryController : Controller
    {
        RERPEntities _db = new RERPEntities();
        // GET: Category
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<tbl_category> itemlist = (from ip in _db.tbl_category
                                               where ip.is_deleted == false
                                               select ip).ToList();
                

                ViewBag.getcate = itemlist;
            return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpGet]
        public ActionResult GetData(int id)
        {
            tbl_category result = _db.tbl_category.Where(x => x.cate_id == id).FirstOrDefault();
            
            var data = new
            {
                cate_id = result.cate_id,
                category_name = result.category_name,
                is_active = result.is_active
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Save(tbl_category data)
        {

            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                tbl_category _pro = new tbl_category();
                _pro.category_name = data.category_name;
                _pro.is_active = data.is_active;
                _pro.is_deleted = false;
                _pro.created_by = user_id;
                _pro.created_pc = UtilityController.GetServerName();
                _pro.created_ip = UtilityController.GetClientIP(Request);
                _pro.created_at = DateTime.Now; 
                _db.tbl_category.Add(_pro);
                _db.SaveChanges();

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        [HttpPost]
        public ActionResult UpdateCategory(tbl_category data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_category.Where(x => x.cate_id == data.cate_id).FirstOrDefault();
                _pro.category_name = data.category_name;
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
            var _client = _db.tbl_category.Where(x => x.cate_id == id).FirstOrDefault();
            _client.is_deleted = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }
        
        
        public List<DropDownModel> GetCategoryList() {
            List<DropDownModel> data = (from cate in _db.tbl_category
                                        where cate.is_deleted == false
                                        select new DropDownModel
                                        {
                                            id = cate.cate_id,
                                            dd_value = cate.category_name
                                        }).ToList();
            return data;

        }
    }
}