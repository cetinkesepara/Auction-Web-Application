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
    
    public partial class uyeler
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public uyeler()
        {
            this.engellenen_uyeler = new HashSet<engellenen_uyeler>();
            this.engellenen_uyeler1 = new HashSet<engellenen_uyeler>();
            this.kargolar = new HashSet<kargolar>();
            this.mesajlasmalar = new HashSet<mesajlasmalar>();
            this.teklif_miktarlari = new HashSet<teklif_miktarlari>();
            this.teklifler = new HashSet<teklifler>();
            this.urunler = new HashSet<urunler>();
        }
    
        public int id { get; set; }
        public string adsoy { get; set; }
        public string email { get; set; }
        public string sifre { get; set; }
        public string telefon { get; set; }
        public string sehir { get; set; }
        public string adres { get; set; }
        public string resim { get; set; }
        public string yetki { get; set; }
        public string durum { get; set; }
        public System.DateTime tarih { get; set; }
        public double bakiye { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<engellenen_uyeler> engellenen_uyeler { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<engellenen_uyeler> engellenen_uyeler1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<kargolar> kargolar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<mesajlasmalar> mesajlasmalar { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<teklif_miktarlari> teklif_miktarlari { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<teklifler> teklifler { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<urunler> urunler { get; set; }
    }
}