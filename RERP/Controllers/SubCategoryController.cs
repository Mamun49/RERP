using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class SubCategoryController : Controller
    {
        // GET: SubCategory
        RERPEntities _db = new RERPEntities();

        CategoryController cate = new CategoryController();
        // GET: Category
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<SubCategoryModel> itemlist = (from ip in _db.tbl_sub_category
                                                   join cat in _db.tbl_category on ip.cate_id equals cat.cate_id
                                                   where ip.is_deleted == false
                                                   select new SubCategoryModel
                                                   {
                                                       sub_cate_id = ip.sub_cate_id,
                                                       cate_name = cat.category_name,
                                                       sub_cate_name = ip.sub_cate_name,
                                                       is_active = ip.is_active
                                                   }).ToList();


                IEnumerable<DropDownModel> catedata = cate.GetCategoryList() as IEnumerable<DropDownModel>;
                ViewBag.getcate = itemlist;
                ViewBag.CategoryLists = catedata;
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpGet]
        public ActionResult GetData(int id)
        {
            tbl_sub_category result = _db.tbl_sub_category.Where(x => x.sub_cate_id == id).FirstOrDefault();

            var data = new
            {
                sub_cate_id = result.sub_cate_id,
                cate_id = result.cate_id,
                sub_cate_name = result.sub_cate_name,
                is_active = result.is_active
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(tbl_sub_category data)
        {

            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                tbl_sub_category _pro = new tbl_sub_category();
                _pro.sub_cate_name = data.sub_cate_name;
                _pro.cate_id = data.cate_id;
                _pro.is_active = data.is_active;
                _pro.is_deleted = false;
                _pro.created_by = user_id;
                _pro.created_pc = UtilityController.GetServerName();
                _pro.created_ip = UtilityController.GetClientIP(Request);
                _db.tbl_sub_category.Add(_pro);
                _db.SaveChanges();

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        [HttpPost]
        public ActionResult UpdateCategory(tbl_sub_category data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_sub_category.Where(x => x.sub_cate_id == data.sub_cate_id).FirstOrDefault();
                _pro.sub_cate_name = data.sub_cate_name;
                _pro.cate_id = data.cate_id;
                _pro.is_active = data.is_active;
                _pro.updated_by = user_id;
                _pro.updated_pc = UtilityController.GetServerName();
                _pro.updated_ip = UtilityController.GetClientIP(Request);
                _db.SaveChanges();
                return Json(new { success = true, Message = "Update Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _client = _db.tbl_sub_category.Where(x => x.sub_cate_id == id).FirstOrDefault();
            _client.is_deleted = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }


        public List<DropDownModel> GetSubCategoryList()
        {
            List<DropDownModel> data = (from cate in _db.tbl_sub_category
                                        where cate.is_deleted == false
                                        select new DropDownModel
                                        {
                                            id = cate.sub_cate_id,
                                            dd_value = cate.sub_cate_name
                                        }).ToList();
            return data;

        }

        public class SubCategoryModel
        {
            public int sub_cate_id { get; set; }
            public int cate_id { get; set; }
            public string cate_name { get; set; }
            public string sub_cate_name { get; set; }
            public bool? is_active { get; set; }
        }
    }
}