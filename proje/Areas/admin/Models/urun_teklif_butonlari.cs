//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace proje.Areas.admin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class urun_teklif_butonlari
    {
        public int id { get; set; }
        public int urun_id { get; set; }
        public int teklif_butonu_id { get; set; }
    
        public virtual teklif_butonlari teklif_butonlari { get; set; }
        public virtual urunler urunler { get; set; }
    }
}