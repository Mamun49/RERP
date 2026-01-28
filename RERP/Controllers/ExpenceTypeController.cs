using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class ExpenceTypeController : Controller
    {
        // GET: ExpenceType
        RERPEntities _db = new RERPEntities();
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<tbl_expense_type> itemlist = (from ip in _db.tbl_expense_type
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
            tbl_expense_type result = _db.tbl_expense_type.Where(x => x.id == id).FirstOrDefault();

            var data = new
            {
                id = result.id,
                ex_name = result.ex_name,
                is_active = result.is_active
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(tbl_expense_type data)
        {

            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                tbl_expense_type _pro = new tbl_expense_type();
                _pro.ex_name = data.ex_name;
                _pro.is_active = data.is_active;
                _pro.is_deleted = false;
                _db.tbl_expense_type.Add(_pro);
                _db.SaveChanges();

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        [HttpPost]
        public ActionResult UpdateCategory(tbl_expense_type data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_expense_type.Where(x => x.id == data.id).FirstOrDefault();
                _pro.ex_name = data.ex_name;
                _pro.is_active = data.is_active;
                _db.SaveChanges();
                return Json(new { success = true, Message = "Update Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _client = _db.tbl_expense_type.Where(x => x.id == id).FirstOrDefault();
            _client.is_deleted = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }


        public List<DropDownModel> GetTypeList()
        {
            List<DropDownModel> data = (from cate in _db.tbl_expense_type
                                        where cate.is_deleted == false
                                        select new DropDownModel
                                        {
                                            id = cate.id,
                                            dd_value = cate.ex_name
                                            }).ToList();
            return data;

        }
    }
}