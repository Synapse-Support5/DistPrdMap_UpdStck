using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistPrdMap_UpdStck.Models
{
    public class UpdateStockModel
    {
        public string ProductERPId { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
    }
}