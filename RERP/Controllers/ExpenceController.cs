using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class ExpenceController : Controller
    {
        // GET: Expence
        RERPEntities _db = new RERPEntities();
        ExpenceTypeController type = new ExpenceTypeController();
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<ExpenseModel> itemlist = (from prod in _db.tbl_expenses
                                            join cate in _db.tbl_expense_type on prod.expence_id equals cate.id
                                            where prod.is_deleted == false
                                            select new
                                            {
                                                prod.expence_id,
                                                prod.note,
                                                prod.remarks,
                                                prod.type_id,
                                                cate.ex_name,
                                                prod.ex_date,
                                                prod.amount
                                            }).AsEnumerable() // Switch to in-memory processing
                            .Select(prod => new ExpenseModel
                            {
                                expence_id = prod.expence_id,
                                type_id = prod.type_id,
                                note = prod.note,
                                remarks = prod.remarks,
                                amount = prod.amount,
                                ex_name = prod.ex_name,
                                ex_date_st = prod.ex_date?.ToString("yyyy-MM-dd") ?? string.Empty
                            }).ToList();


                ViewBag.getcate = itemlist;
                IEnumerable<DropDownModel> exptype = type.GetTypeList() as IEnumerable<DropDownModel>;
                ViewBag.TypeLists = exptype;
                
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpGet]
        public ActionResult GetData(int id)
        {
            var result = (from p in _db.tbl_expenses
                          where p.expence_id == id && p.is_deleted == false
                          select new
                          {
                              p.expence_id,
                              p.type_id,
                              p.note,
                              p.remarks,
                              p.amount,
                              p.ex_date
                          }).AsEnumerable() // Switch to in-memory processing
              .Select(p => new ExpenseModel
              {
                  expence_id = p.expence_id,
                  type_id = p.type_id,
                  note = p.note,
                  remarks = p.remarks,
                  amount = p.amount,
                  ex_date_st = p.ex_date?.ToString("yyyy-MM-dd") ?? string.Empty,
                  ex_date = p.ex_date
              }).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(tbl_expenses data)
        {

            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                tbl_expenses _pro = new tbl_expenses();
                _pro.type_id = data.type_id;
                _pro.note = data.note;
                _pro.amount = data.amount;
                _pro.ex_date = data.ex_date;
                _pro.remarks = data.remarks;
                _pro.is_deleted = false;
                _db.tbl_expenses.Add(_pro);
                _db.SaveChanges();

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        [HttpPost]
        public ActionResult UpdateCategory(tbl_expenses data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_expenses.Where(x => x.expence_id == data.expence_id).FirstOrDefault();
                _pro.type_id = data.type_id;
                _pro.note = data.note;
                _pro.amount = data.amount;
                _pro.ex_date = data.ex_date;
                _pro.remarks = data.remarks;
                _db.SaveChanges();
                return Json(new { success = true, Message = "Update Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _client = _db.tbl_expenses.Where(x => x.expence_id == id).FirstOrDefault();
            _client.is_deleted = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }
        public class ExpenseModel
        {
            public int expence_id { get; set; }
            public int type_id { get; set; }
            public string note { get; set; }
            public string remarks { get; set; }
            public Nullable<decimal> amount { get; set; }
            public Nullable<System.DateTime> ex_date { get; set; }
            public string ex_date_st { get; set; }
            public string ex_name { get; set; }
        }

        public ActionResult Dailyreport()
        {
            List<ExpenseModel> itemlist = (from prod in _db.tbl_expenses
                                           join cate in _db.tbl_expense_type on prod.expence_id equals cate.id
                                           where prod.is_deleted == false && prod.ex_date == DateTime.Today
                                           select new
                                           {
                                               prod.expence_id,
                                               prod.note,
                                               prod.remarks,
                                               prod.type_id,
                                               cate.ex_name,
                                               prod.ex_date,
                                               prod.amount
                                           }).AsEnumerable() // Switch to in-memory processing
                            .Select(prod => new ExpenseModel
                            {
                                expence_id = prod.expence_id,
                                type_id = prod.type_id,
                                note = prod.note,
                                remarks = prod.remarks,
                                amount = prod.amount,
                                ex_name = prod.ex_name,
                                ex_date_st = prod.ex_date?.ToString("yyyy-MM-dd") ?? string.Empty
                            }).ToList();
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Monthlyreport(int month)
        {
            List<ExpenseModel> itemlist = (from prod in _db.tbl_expenses
                                           join cate in _db.tbl_expense_type on prod.expence_id equals cate.id
                                           where prod.is_deleted == false && prod.ex_date.Value.Month == month
                                           select new
                                           {
                                               prod.expence_id,
                                               prod.note,
                                               prod.remarks,
                                               prod.type_id,
                                               cate.ex_name,
                                               prod.ex_date,
                                               prod.amount
                                           }).AsEnumerable() // Switch to in-memory processing
                            .Select(prod => new ExpenseModel
                            {
                                expence_id = prod.expence_id,
                                type_id = prod.type_id,
                                note = prod.note,
                                remarks = prod.remarks,
                                amount = prod.amount,
                                ex_name = prod.ex_name,
                                ex_date_st = prod.ex_date?.ToString("yyyy-MM-dd") ?? string.Empty
                            }).ToList();
            return Json(itemlist, JsonRequestBehavior.AllowGet);
        }
    }
}