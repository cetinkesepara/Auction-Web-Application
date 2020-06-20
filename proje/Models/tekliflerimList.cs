using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proje.Models
{
    public class tekliflerimList
    {
        public string urun_adi { get; set; }
        public Nullable<System.DateTime> bitis_tarihi { get; set; }
        public int urun_id { get; set; }
        public double toplam { get; set; }
        public int uye_id { get; set; }
    }
}