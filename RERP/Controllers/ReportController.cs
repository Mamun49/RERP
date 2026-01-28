using RERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RERP.Controllers
{
    public class ReportController : Controller
    {
        RERPEntities _db = new RERPEntities();
        // GET: Report
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<ProductModel> itemlist = (from prod in _db.tbl_product
                                               join cate in _db.tbl_category on prod.product_category equals cate.cate_id
                                               where prod.is_delete == false
                                               select new ProductModel
                                               {
                                                   product_id = prod.product_id,
                                                   dp = prod.dp,
                                                   prod_name = prod.prod_name,
                                                   product_code = prod.product_code,
                                                   category_name = cate.category_name,
                                                   remaining_total_qty = prod.remaining_total_qty,
                                                   buying_price = prod.buying_price,
                                                   sale_price = prod.sale_price,
                                                   is_active = prod.is_active
                                               }).ToList();
                ViewBag.getprod = itemlist;
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
        public ActionResult SellReport()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                List<SellModel> itemlist = (from prod in _db.tbl_sell
                                            where prod.is_deleted == false
                                            select new SellModel
                                            {
                                                sell_id = prod.sell_id,
                                                customer_name = prod.customer_name,
                                                selling_date = prod.selling_date,
                                                total_value = prod.total_value,
                                                remarks = prod.remarks
                                            }).ToList();
                ViewBag.getprod = itemlist;
                return View();
            }
            else { return RedirectToRoute("login"); }
        }

        public ActionResult GetReport(int product_id)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                var result = (from p in _db.tbl_product
                              join pd in _db.tbl_product_details on p.product_id equals pd.product_id
                              join c in _db.tbl_category on p.product_category equals c.cate_id
                              join cl in _db.tbl_color on pd.prod_color equals cl.color_id into colorGroup
                              from color in colorGroup.DefaultIfEmpty()
                              join s in _db.tbl_size on pd.prod_size equals s.size_id into sizeGroup
                              from size in sizeGroup.DefaultIfEmpty()
                              where p.product_id == product_id && pd.is_deleted == false
                              group new { p, c, pd, color, size } by new { p, c } into g
                              select new ProductModel
                              {
                                  product_id = g.Key.p.product_id,
                                  product_code = g.Key.p.product_code,
                                  product_category = g.Key.p.product_category,
                                  category_name = g.Key.c.category_name,
                                  prod_name = g.Key.p.prod_name,
                                  prod_type = g.Key.p.prod_type,
                                  total_qty = g.Key.p.total_qty,
                                  remaining_total_qty = g.Key.p.remaining_total_qty,
                                  buying_price = g.Key.p.buying_price,
                                  sale_price = g.Key.p.sale_price,
                                  manufacturername = g.Key.p.manufacturername,
                                  remarks = g.Key.p.remarks,
                                  is_active = g.Key.p.is_active,
                                  other_costing = g.Key.p.other_costing,
                                  discont_rate = g.Key.p.discont_rate,
                                  discount_type = g.Key.p.discount_type,
                                  dp = g.Key.p.dp,
                                  ProductDetails = g.Select(x => new ProductDetail
                                  {
                                      product_details_id = x.pd.product_details_id,
                                      product_id = x.pd.product_id,
                                      prod_code = x.pd.prod_code,
                                      prod_color_name = x.color.color_name,
                                      prod_size_name = x.size.size_name,
                                      prod_qty = x.pd.prod_qty,
                                      prod_remain_qty = x.pd.prod_remain_qty,
                                      prod_selling_qty = x.pd.prod_selling_qty
                                  }).ToList()
                              }).FirstOrDefault();

                if (result == null)
                {
                    return HttpNotFound($"Product with ID {product_id} not found.");
                }

                return View(result);
            }
            else { return RedirectToRoute("login"); }
        }
        public ActionResult GetSellReport(int sell_id)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                var result = (from p in _db.tbl_sell
                              join pd in _db.tbl_sell_details on p.sell_id equals pd.sell_id
                              join cl in _db.tbl_color on pd.color_id equals cl.color_id into colorGroup
                              from color in colorGroup.DefaultIfEmpty()
                              join s in _db.tbl_size on pd.size_id equals s.size_id into sizeGroup
                              from size in sizeGroup.DefaultIfEmpty()
                              where p.sell_id == sell_id && p.is_deleted == false
                              group new { p, pd, color, size } by new { p } into g
                              select new SellModel
                              {
                                  customer_name = g.Key.p.customer_name,
                                  selling_date = g.Key.p.selling_date,
                                  remarks = g.Key.p.remarks,
                                  total_value = g.Key.p.total_value,
                                  SellDetails = g.Select(x => new SellDetail
                                  {
                                      sell_details_id = x.pd.sell_details_id,
                                      item_id = x.pd.item_id,
                                      item_code = x.pd.item_code,
                                      color_name = x.color.color_name,
                                      size_name = x.size.size_name,
                                      qty = x.pd.qty,
                                      selling_price = x.pd.price
                                  }).ToList()
                              }).FirstOrDefault();

                if (result == null)
                {
                    return HttpNotFound($"Product with ID {sell_id} not found.");
                }

                return View(result);
            }
            else { return RedirectToRoute("login"); }
        }
        public ActionResult ExpenceReport()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
    }
}