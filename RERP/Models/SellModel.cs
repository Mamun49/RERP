using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RERP.Models
{
    public class SellModel
    {
        public int sell_id { get; set; }    
        public string customer_name { get; set; }
        public DateTime? selling_date { get; set; }
        public string remarks { get; set; }
        public decimal? total_value { get; set; }    
        public List<SellDetail> SellDetails { get; set; }
    }
    public class SellDetail { 
    public int sell_details_id { get; set; }
        public int sell_id { get; set; }
        public int? item_id { get; set; }
        public int? item_details_id { get; set; }
        public int color_id { get; set; }
        public int size_id { get; set; }
        public string item_code { get; set; }
        public string dp { get; set; }
        public int? qty { get; set; }
        public decimal? selling_price { get; set; }
        public string remarks { get; set; }
        public string color_name { get; set; }
        public string size_name { get; set; }
        public string cate_name { get; set; }

    }
}