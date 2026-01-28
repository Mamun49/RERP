using RERP.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace RERP.Controllers
{
    public class SellController : Controller
    {
        RERPEntities _db = new RERPEntities();
        ColorController color = new ColorController();
        SizeController size = new SizeController();
        ProductController product = new ProductController();
        // GET: Sell
        public ActionResult Index()
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                // IEnumerable<DropDownModel> colordata = color.GetColorList();
                //IEnumerable<DropDownModel> sizedata = size.GetSizeList();
                IEnumerable<DropDownModel> proddata = product.GetProdList();
            
            //ViewBag.ColorLists = colordata;
           // ViewBag.SizeLists = sizedata;
            ViewBag.ProdLists = proddata;
            return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpPost]
        public ActionResult SaveSell(SellModel data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {     
                tbl_sell _pro = new tbl_sell
                {
                    customer_name = data.customer_name,
                    selling_date = data.selling_date,
                    remarks = data.remarks,
                    total_value = Convert.ToInt32(data.total_value),
                    is_deleted = false,
                    status = true,
                    created_by = user_id,
                    created_pc = UtilityController.GetServerName(),
                    created_ip = UtilityController.GetClientIP(Request),
                    created_at = DateTime.Now
                };

                _db.tbl_sell.Add(_pro);
                _db.SaveChanges();

                foreach (var details in data.SellDetails)
                {
                    var product_details = _db.tbl_product_details
    .Where(pd => pd.product_id == details.item_id && pd.prod_color == details.color_id && pd.prod_size == details.size_id)
    .OrderByDescending(pd => pd.product_details_id)
    .Select(pd => pd.product_details_id)
    .FirstOrDefault();
                    if (details.qty != 0)
                    {
                        var item_data = _db.tbl_product_details.Where(x => x.product_details_id == product_details).FirstOrDefault();
                        var product = _db.tbl_product.Where(x => x.product_id == details.item_id).FirstOrDefault();
                        var remainqty = item_data.prod_remain_qty - details.qty;
                        item_data.prod_remain_qty = remainqty;
                        item_data.prod_selling_qty = details.qty + item_data.prod_selling_qty;
                        var total_remain_qty = product.remaining_total_qty - details.qty;
                        product.remaining_total_qty = total_remain_qty;
                        product.selling_total_qty = product.selling_total_qty + details.qty; 
                        _db.SaveChanges();
                        tbl_sell_details _pd = new tbl_sell_details
                        {
                            sell_id = _pro.sell_id,
                            item_id = details.item_id,
                            color_id = details.color_id,
                            size_id = details.size_id,
                            qty = details.qty,
                            item_code = item_data.prod_code,
                            remarks = details.remarks,
                            price = details.selling_price,
                            item_details_id = product_details,
                            created_by = user_id,
                            created_pc = UtilityController.GetServerName(),
                            created_ip = UtilityController.GetClientIP(Request),
                            created_at = DateTime.Now
                        };
                        _db.tbl_sell_details.Add(_pd);
                        _db.SaveChanges();
                    }
                }

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }
        public ActionResult SellList() {
           
                int user_id;
                if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
                {
                List<SellDetail> itemlist = (from pd in _db.tbl_sell_details
                                             join cl in _db.tbl_color on pd.color_id equals cl.color_id into colorGroup
                                             from color in colorGroup.DefaultIfEmpty()
                                             join s in _db.tbl_size on pd.size_id equals s.size_id into sizeGroup
                                             from size in sizeGroup.DefaultIfEmpty()
                                             select new SellDetail
                                             {
                                                 //dp = _db.tbl_product_details.Where(x => x.product_id == pd.item_id).Select(x => x.prod_image).FirstOrDefault(),
                                                       sell_details_id = pd.sell_details_id,
                                                 item_details_id = pd.item_details_id,
                                                 item_code = pd.item_code,
                                                 color_name = color.color_name,
                                                 size_name = size.size_name,
                                                 qty = pd.qty,
                                             }).ToList();
                    ViewBag.getprod = itemlist;
                   
                    return View();
                }
                else { return RedirectToRoute("login"); }
            
        }
        [HttpGet]
        public ActionResult GetData(int id)
        {
            tbl_sell_details result = _db.tbl_sell_details.Where(x => x.sell_details_id == id).FirstOrDefault();

            var data = new
            {
                sell_details_id = result.sell_details_id,
                product = result.sell_details_id,
                qty = result.qty
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
      
        public ActionResult ReturnQty(tbl_sell_details data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {

                var _pro = _db.tbl_sell_details.Where(x => x.sell_details_id == data.sell_details_id).FirstOrDefault();
                _pro.qty = data.qty;
                _pro.updated_by = user_id;
                _pro.updated_pc = UtilityController.GetServerName();
                _pro.updated_at = DateTime.Now;
                _pro.updated_ip = UtilityController.GetClientIP(Request);
                _db.SaveChanges();
                var item_data = _db.tbl_product_details.Where(x => x.product_details_id == _pro.item_details_id).FirstOrDefault();
                decimal? item_add = item_data.prod_remain_qty + data.qty;
                decimal? remove_sale = item_data.prod_selling_qty-data.qty;
                item_data.prod_remain_qty = item_add;
                item_data.prod_selling_qty = remove_sale;
                _db.SaveChanges();
                return Json(new { success = true, Message = "Update Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
    }
}