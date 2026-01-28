using RERP.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Drawing.Printing;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;

namespace RERP.Controllers
{
    public class ProductController : Controller
    {
        RERPEntities _db = new RERPEntities();
        CategoryController cate = new CategoryController();
        ColorController color = new ColorController();
        SizeController size = new SizeController();

        // GET: Product
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
                                                   is_active = prod.is_active,
                                                   remarks = prod.remarks
                                               }).ToList();
                ViewBag.getprod = itemlist;
                IEnumerable<DropDownModel> catedata = cate.GetCategoryList() as IEnumerable<DropDownModel>;
                IEnumerable<DropDownModel> colordata = color.GetColorList();
                IEnumerable<DropDownModel> sizedata = size.GetSizeList();
                ViewBag.CategoryLists = catedata;
                ViewBag.ColorLists = colordata;
                ViewBag.SizeLists = sizedata;
                return View();
            }
            else { return RedirectToRoute("login"); }
        }
        [HttpGet]
        public ActionResult GetProductData(int product_id)
        {
            var result = (from p in _db.tbl_product
                          join pd in _db.tbl_product_details on p.product_id equals pd.product_id
                          join c in _db.tbl_category on p.product_category equals c.cate_id
                          where p.product_id == product_id && pd.is_deleted == false
                          group pd by new { p, c } into g
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
                              last_price = g.Key.p.last_price,
                              manufacturername = g.Key.p.manufacturername,
                              remarks = g.Key.p.remarks,
                              is_active = g.Key.p.is_active,
                              other_costing = g.Key.p.other_costing,
                              discont_rate = g.Key.p.discont_rate,
                              discount_type = g.Key.p.discount_type,
                              dp = g.Key.p.dp,
                              ProductDetails = g.Select(pd => new ProductDetail
                              {
                                  product_details_id = pd.product_details_id,
                                  product_id = pd.product_id,
                                  prod_code = pd.prod_code,
                                  prod_size = pd.prod_size,
                                  prod_color = pd.prod_color,
                                  prod_qty = pd.prod_qty,
                                  prod_remain_qty = pd.prod_remain_qty,
                                  prod_selling_qty = pd.prod_selling_qty
                              }).ToList()
                          }).FirstOrDefault();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveProduct(ProductModel data)
        {
            string product_code = GenerateUniqueCode();
            //decimal? totalQuantity = data.ProductDetails.Sum(pd => pd.prod_qty);
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                var file = Request.Files["dp"];
                string imageUrl = null;

                if (file != null && file.ContentLength > 0)
                {
                    string imagePath = Server.MapPath("~/Files/");

                    // Ensure the directory exists
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    string fullPath = Path.Combine(imagePath, fileName);

                    // Save the file
                    file.SaveAs(fullPath);
                    imageUrl = "/Files/" + fileName;
                }
                tbl_product _pro = new tbl_product
                {
                    buying_price = data.buying_price,
                    sale_price = data.sale_price,
                    last_price = data.last_price,
                    product_category = data.product_category,
                    prod_name = data.prod_name,
                    prod_type = data.prod_type,
                    manufacturername = data.manufacturername,
                    remarks = data.remarks,
                    product_code = product_code, // Assuming you handle this elsewhere
                    total_qty = data.ProductDetails.Sum(pd => pd.prod_qty),
                    remaining_total_qty = data.ProductDetails.Sum(pd => pd.prod_qty),
                    dp = imageUrl,
                    other_costing = data.other_costing,
                    discount_type = data.discount_type,
                    discont_rate = data.discont_rate,
                    is_active = data.is_active,
                    is_delete = false,
                    created_by = user_id,
                    created_pc = UtilityController.GetServerName(),
                    created_ip = UtilityController.GetClientIP(Request),
                    created_at = DateTime.Now
                };

                _db.tbl_product.Add(_pro);
                _db.SaveChanges();

                foreach (var details in data.ProductDetails)
                {
                    if (details.prod_qty != 0)
                    {
                        tbl_product_details _pd = new tbl_product_details
                        {
                            product_id = _pro.product_id,
                            prod_code = product_code, // Assuming you handle this elsewhere
                            prod_size = details.prod_size,
                            prod_color = details.prod_color,
                            prod_qty = details.prod_qty,
                            prod_remain_qty = details.prod_qty,
                            prod_remarks = details.prod_remarks,
                            prod_price = data.sale_price,
                            is_deleted = false,
                            created_by = user_id,
                            created_pc = UtilityController.GetServerName(),
                            created_ip = UtilityController.GetClientIP(Request),
                            created_at = DateTime.Now
                        };
                        _db.tbl_product_details.Add(_pd);
                        _db.SaveChanges();
                    }
                }

                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }

        }

        //public int GenerateUniqueNumber()
        //{
        //    var random = new Random();

        //    while (true)
        //    {
        //        var randomNumber = random.Next(10000, 100000); // Generate a random number between 10000 and 99999.

        //        Check if the number already exists in the table.
        //        var exists = _db.tbl_product.Any(x => x.product_code == randomNumber);

        //        if (!exists)
        //        {
        //            If the number doesn't exist in the table, return it.
        //            return randomNumber;
        //        }
        //    }
        //}
        public string GenerateUniqueCode()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz"; // Allowed characters

            while (true)
            {
                // Generate a random string of 6 characters (you can adjust the length)
                var randomCode = new string(Enumerable.Repeat(chars, 4)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                // Check if the code already exists in the table
                var exists = _db.tbl_product.Any(x => x.product_code == randomCode);

                if (!exists)
                {
                    // If the code doesn't exist, return it
                    return randomCode;
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int product_id)
        {
            var _client = _db.tbl_product.Where(x => x.product_id == product_id).FirstOrDefault();
            _client.is_delete = true;
            _db.SaveChanges();
            return Json(new { success = true, Message = "Delete Successfully!" });
        }
        [HttpPost]
        public ActionResult UpdateProduct(ProductModel data)
        {
            int user_id;
            if (Session["ID"] != null && int.TryParse(Session["ID"].ToString(), out user_id))
            {
                var file = Request.Files["dp"];
                string imageUrl = null;

                if (file != null && file.ContentLength > 0)
                {
                    string imagePath = Server.MapPath("~/Files/");

                    // Ensure the directory exists
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    string fullPath = Path.Combine(imagePath, fileName);

                    // Save the file
                    file.SaveAs(fullPath);
                    imageUrl = "/Files/" + fileName;
                }
                var _prod = _db.tbl_product.Where(x => x.product_id == data.product_id).FirstOrDefault();
                _prod.prod_name = data.prod_name;
                _prod.buying_price = data.buying_price;
                _prod.sale_price = data.sale_price;
                _prod.last_price = data.last_price;
                _prod.product_category = data.product_category;
                _prod.prod_type = data.prod_type;
                _prod.manufacturername = data.manufacturername;
                _prod.remarks = data.remarks;
                _prod.total_qty = data.ProductDetails.Sum(pd => pd.prod_qty);
                _prod.remaining_total_qty = data.ProductDetails.Sum(pd => pd.prod_qty);
                _prod.dp = imageUrl;
                _prod.other_costing = data.other_costing;
                _prod.discont_rate = data.discont_rate;
                _prod.discount_type = data.discount_type;
                _prod.is_active = data.is_active;
                _prod.updated_by = user_id;
                _prod.updated_pc = UtilityController.GetServerName();
                _prod.updated_at = DateTime.Now;
                _prod.updated_ip = UtilityController.GetClientIP(Request);
                _db.SaveChanges();
                var product_code = _db.tbl_product.Where(x => x.product_id == data.product_id).Select(x => x.product_code).FirstOrDefault();

                var reqdetails = _db.tbl_product_details.Where(x => x.product_id == data.product_id).Select(x => x.product_details_id).ToList();
                foreach (var dat in data.ProductDetails)
                {
                    if (dat.prod_qty != null)
                    {
                        if (reqdetails.Contains(dat.product_details_id))
                        {
                            var req_details_id = dat.product_details_id;

                            var details = _db.tbl_product_details.Where(x => x.product_details_id == req_details_id).FirstOrDefault();
                            details.prod_color = dat.prod_color;
                            details.prod_qty = dat.prod_qty;
                            details.prod_size = dat.prod_size;
                            details.prod_remain_qty = dat.prod_qty;
                            details.updated_by = user_id;
                            details.updated_pc = UtilityController.GetServerName();
                            details.updated_at = DateTime.Now;
                            details.updated_ip = UtilityController.GetClientIP(Request);
                            _db.SaveChanges();


                        }
                        else
                        {

                            tbl_product_details _pd = new tbl_product_details
                            {

                                product_id = data.product_id,
                                prod_code = product_code, // Assuming you handle this elsewhere
                                prod_size = dat.prod_size,
                                prod_color = dat.prod_color,
                                prod_qty = dat.prod_qty,
                                prod_remain_qty = dat.prod_qty,
                                prod_remarks = dat.prod_remarks,
                                is_deleted = false,
                                created_by = user_id,
                                created_pc = UtilityController.GetServerName(),
                                created_ip = UtilityController.GetClientIP(Request),
                                created_at = DateTime.Now
                            };
                            _db.tbl_product_details.Add(_pd);
                            _db.SaveChanges();

                        }
                    }

                }
                foreach (var product_details_id in reqdetails)
                {
                    if (!data.ProductDetails.Any(dat => dat.product_details_id == product_details_id))
                    {
                        var _client = _db.tbl_product_details.Where(x => x.product_details_id == product_details_id).FirstOrDefault();
                        _client.is_deleted = true;
                        _db.SaveChanges();

                    }
                }



                return Json(new { success = true, Message = "Data Save Successfully!" });
            }
            else { return RedirectToRoute("login"); }
        }
        public List<DropDownModel> GetProdList()
        {
            RERPEntities _db = new RERPEntities();
            var products = _db.tbl_product
                .Where(p => p.remaining_total_qty != 0)
                .ToList();

            var data = products.Select(p => new DropDownModel
            {
                dd_value = $"{p.product_code} | {p.prod_name} | Rem. Qty: {p.remaining_total_qty}",
                id = p.product_id
            }).ToList();

            return data;
        }
        [HttpGet]
        [Route("Product/GetProductColorData/{product_id}")]
        public JsonResult GetProductColorData(int product_id)
        {
            var result = _db.tbl_product_details
                .Where(pd => pd.product_id == product_id)
                .Join(_db.tbl_color,
                      pd => pd.prod_color,
                      c => c.color_id,
                      (pd, c) => new DropDownModel
                      {
                          id = pd.prod_color,
                          dd_value = c.color_name
                      })
                .Distinct()
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Product/GetProductSizeData")]
        public JsonResult GetProductSizeData(int product_id, int color_id)
        {
            var result = _db.tbl_product_details
                .Where(pd => pd.product_id == product_id && pd.prod_color == color_id)
                .Join(_db.tbl_size,
                      pd => pd.prod_size,
                      c => c.size_id,
                      (pd, c) => new DropDownModel
                      {
                          id = pd.prod_size,
                          dd_value = c.size_name
                      })
                .Distinct()
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Route("Product/GetProductQtyData")]
        public JsonResult GetProductQtyData(int product_id, int color_id, int size_id)
        {
            var result = _db.tbl_product_details
    .Where(pd => pd.product_id == product_id && pd.prod_color == color_id && pd.prod_size == size_id)
    .OrderByDescending(pd => pd.product_details_id)
    .Select(pd => pd.prod_remain_qty)
    .FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public void PrintBarcodeAndPrice(int product_id)
        {
            var product = _db.tbl_product.Where(x => x.product_id == product_id).FirstOrDefault();

            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            // Define the PrintDocument
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += (sender, e) =>
            {
                // Define dimensions in millimeters
                float widthMm = 35;
                float heightMm = 28;

                // Convert dimensions to pixels (DPI = 300)
                float dpi = 300;
                float widthPx = widthMm / 25.4f * dpi;
                float heightPx = heightMm / 25.4f * dpi;

                // Set the paper size
                e.PageSettings.PaperSize = new PaperSize("Custom", (int)widthPx, (int)heightPx);

                // Generate Barcode
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 60, // Barcode height
                        Width = 180 // Barcode width
                    }
                };
                Bitmap barcode = writer.Write(product.product_code);

                // Draw Barcode
                float barcodeX = 10; // Position from the left
                float barcodeY = 10; // Position from the top
                e.Graphics.DrawImage(barcode, barcodeX, barcodeY);

                // Draw Selling Price
                string sellingPriceText = $"Price: {product.sale_price:C}";
                Font font = new Font("Arial", 10, FontStyle.Bold);
                Brush brush = Brushes.Black;
                float textX = 10;
                float textY = barcodeY + barcode.Height + 10; // Below the barcode
                e.Graphics.DrawString(sellingPriceText, font, brush, textX, textY);
            };

            // Print
            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Printing failed: {ex.Message}");
            }
        }
        [HttpGet]
        public ActionResult GenerateBarcode(int product_id)
        {
            // Fetch product details with color, size, and quantity information for each item
            var product = (from p in _db.tbl_product
                           join pd in _db.tbl_product_details on p.product_id equals pd.product_id
                           join c in _db.tbl_category on p.product_category equals c.cate_id
                           join cl in _db.tbl_color on pd.prod_color equals cl.color_id
                           join s in _db.tbl_size on pd.prod_size equals s.size_id
                           where p.product_id == product_id && pd.is_deleted == false
                           group new { p, pd, cl, s } by new { p, c } into g
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
                               last_price = g.Key.p.last_price,
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
                                   prod_size = x.pd.prod_size,
                                   prod_color = x.pd.prod_color,
                                   prod_color_name = x.cl.color_name,
                                   prod_size_name = x.s.size_name,
                                   prod_qty = x.pd.prod_qty,
                                   prod_remain_qty = x.pd.prod_remain_qty,
                                   prod_selling_qty = x.pd.prod_selling_qty
                               }).ToList()
                           }).FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound($"Product with ID {product_id} not found.");
            }

            // Generate multiple barcodes for each item with color, size, and quantity
            var barcodes = product.ProductDetails.SelectMany(detail =>
            {
                var barcodesForItem = new List<object>();
                for (int i = 0; i < detail.prod_remain_qty; i++)
                {
                    var barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.CODE_128,
                        Options = new ZXing.Common.EncodingOptions
                        {
                            Width = 200,
                            Height = 50,
                            Margin = 1
                        }
                    };

                    using (Bitmap bitmap = barcodeWriter.Write(detail.prod_code))
                    {
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                            var base64Barcode = Convert.ToBase64String(stream.ToArray());

                            barcodesForItem.Add(new
                            {
                                success = true,
                                barcode = $"data:image/png;base64,{base64Barcode}",
                                prod_name = product.prod_name,
                                color = detail.prod_color_name,
                                size = detail.prod_size_name,
                                sale_price = product.sale_price
                            });
                        }
                    }
                }
                return barcodesForItem;
            }).ToList();

            return Json(barcodes, JsonRequestBehavior.AllowGet);
        }


    }
}