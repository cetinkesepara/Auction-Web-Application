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
    
    public partial class galeriler
    {
        public int id { get; set; }
        public int urun_id { get; set; }
        public string resim { get; set; }
    
        public virtual urunler urunler { get; set; }
    }
}
