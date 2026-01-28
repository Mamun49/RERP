using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RERP.Models
{
    public class ProductModel
    {
        [Key]
        public int product_id { get; set; }

        public int? com_id { get; set; }


        public string product_code { get; set; }

        public int? product_category { get; set; }
        public string category_name { get; set; }

        public int? product_sub_category { get; set; }


        public string prod_name { get; set; }

        public string prod_type { get; set; }

        public decimal? total_qty { get; set; }

        public decimal? selling_total_qty { get; set; }

        public decimal? remaining_total_qty { get; set; }

        public decimal? total_buying_price { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string dp { get; set; }

        public decimal? sale_price { get; set; }
        public decimal? last_price { get; set; }

        public decimal? buying_price { get; set; }

        public decimal? other_costing { get; set; }


        public string discount_type { get; set; }
        public string remarks { get; set; }
        public string manufacturername { get; set; }

        public decimal? discont_rate { get; set; }

        public bool? is_active { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }

    }
    public class ProductDetail
    {
        [Key]

        public int product_details_id { get; set; }

        public int? product_id { get; set; }


        public string prod_code { get; set; }
        public int? prod_size { get; set; }
        public string prod_size_name { get; set; }

        public int? prod_color { get; set; }
        public string prod_color_name { get; set; }

        public decimal? prod_qty { get; set; }

        public decimal? prod_selling_qty { get; set; }

        public decimal? prod_remain_qty { get; set; }
        public decimal? prod_price { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string prod_image { get; set; }
        public string prod_remarks { get; set; }
    }
}