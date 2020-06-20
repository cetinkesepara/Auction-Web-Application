using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    [LoginAuthorize]
    public class TekliflerController : Controller
    {

        public ActionResult FirmaTeklifler()
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "FirmaTeklifler";
            projeEntities1 db = new projeEntities1();
            DateTime localTime = DateTime.Now;

            var urunler = (from u in db.urunler where u.bitis_tarihi <= localTime && u.teklif_durumu == "Evet" && (u.uyeler.yetki == "Admin" || u.uyeler.yetki == "Yönetici") select u).ToList();
            if (urunler.Count() > 0)
            {
                foreach (var item in urunler)
                {
                    item.teklif_durumu = "Hayır";
                    db.SaveChanges();
                }
            }

            var teklifler = from t in db.teklifler where t.urunler.bitis_tarihi <= localTime && t.urunler.teklif_durumu == "Hayır" && (t.urunler.uyeler.yetki == "Admin" || t.urunler.uyeler.yetki == "Yönetici") select t;

            return View(teklifler.ToList().OrderBy(x => x.urunler.bitis_tarihi));
        }

        public ActionResult Firma_teklif_islem(int id)
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "FirmaTeklifler";
            projeEntities1 db = new projeEntities1();
            DateTime localTime = DateTime.Now;

            var teklifler = from t in db.teklifler where t.id == id && t.urunler.bitis_tarihi <= localTime && t.urunler.teklif_durumu == "Hayır" && (t.urunler.uyeler.yetki == "Admin" || t.urunler.uyeler.yetki == "Yönetici") select t;

            return View(teklifler.First());
        }

        [HttpPost]
        public ActionResult Firma_teklif_islem(FormCollection col, int teklif_id)
        {
            projeEntities1 db = new projeEntities1();
            var teklif = (from t in db.teklifler where t.id == teklif_id && (t.urunler.uyeler.yetki == "Admin" || t.urunler.uyeler.yetki == "Yönetici") select t).First();
            int teklif_islem = Convert.ToInt32(col["teklif_islem"]);

            if (teklif_islem == 1)
            {
                // Üyelerin bakiyelerini geri yükle
                var group = (from gr in db.teklif_miktarlari
                             where gr.urun_id == teklif.urun_id
                             group gr by new { gr.uye_id }
                             into g
                             select new { g.Key.uye_id, Toplam = g.Sum(a => a.teklif_butonlari.miktar) }).ToList();

                var uyeler = (from u in db.uyeler select u).ToList();

                foreach (var uye in group)
                {
                    foreach (var item in uyeler)
                    {
                        if (uye.uye_id == item.id)
                        {
                            item.bakiye += uye.Toplam;
                            db.SaveChanges();
                            break;
                        }
                    }
                }

                // Teklif_miktarları tablosunu temizle
                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(c => c.urun_id == teklif.urun_id));

                // Urunu resetle
                var urun = (from u in db.urunler where u.id == teklif.urun_id select u).First();
                urun.baslangic_tarihi = null;
                urun.bitis_tarihi = null;
                urun.yayin_durumu = "Hayır";

                // Urun teklif butonlarını sil
                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(c => c.urun_id == teklif.urun_id));

                // Teklifler tablosunu temizle
                db.teklifler.RemoveRange(db.teklifler.Where(c => c.id == teklif.id));
                db.SaveChanges();
            }
            else if (teklif_islem == 2)
            {
                // Kaybeden Üyelerin bakiyelerini geri yükle ve mesaj gönder
                var group = (from gr in db.teklif_miktarlari
                             where gr.urun_id == teklif.urun_id && gr.uye_id != teklif.uye_id
                             group gr by new { gr.uye_id }
                             into g
                             select new { g.Key.uye_id, Toplam = g.Sum(a => a.teklif_butonlari.miktar) }).ToList();

                var uyeler = (from u in db.uyeler where u.id != teklif.uye_id select u).ToList();

                foreach (var uye in group)
                {
                    foreach (var item in uyeler)
                    {
                        if (uye.uye_id == item.id)
                        {
                            item.bakiye += uye.Toplam;

                            var mesaj = new mesajlasmalar();
                            mesaj.uye_id = uye.uye_id;
                            mesaj.tarih = DateTime.Now;
                            mesaj.mesaj = "<p> '" + teklif.urunler.adi + "' isimli ürünün açık arttırmasını kazanamadınız. Yaptığınız teklifler bakiyenize geri yüklenmiştir.</p>";
                            mesaj.durum = "Yeni";
                            mesaj.tur = "Alınan";
                            mesaj.mesaj_turu = "Açıkarttırma";

                            db.mesajlasmalar.Add(mesaj);
                            db.SaveChanges();
                            break;
                        }
                    }
                }

                // Kazanan kişi adına kargo oluştur ve mesaj gönder

                var kargo = new kargolar();
                kargo.tur = "Firma-Üye";
                kargo.durum = "Kargolama İşlemi Bekleniyor";
                kargo.tarih = DateTime.Now;
                kargo.urun_id = teklif.urun_id;
                kargo.uye_id = teklif.uye_id;
                db.kargolar.Add(kargo);

                var mesaj2 = new mesajlasmalar();
                mesaj2.uye_id = teklif.uye_id;
                mesaj2.tarih = DateTime.Now;
                mesaj2.mesaj = "<p> '" + teklif.urunler.adi + "' isimli ürünün açık arttırmasını kazandınız. 'Alınan Kargolarım' sayfasından ürünü takip edebilirsiniz.</p>";
                mesaj2.durum = "Yeni";
                mesaj2.tur = "Alınan";
                mesaj2.mesaj_turu = "Açıkarttırma";

                db.mesajlasmalar.Add(mesaj2);
                db.SaveChanges();

                // Teklif Miktarları tablosunu temizle
                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(c => c.urun_id == teklif.urun_id));

                // Urunu pasif yap
                var urun = (from u in db.urunler where u.id == teklif.urun_id select u).First();
                urun.durum = "Pasif";

                // Urun teklif butonlarını sil
                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(c => c.urun_id == teklif.urun_id));

                // Teklifler tablosu durumunu Onaylandı yap
                teklif.durum = "Onaylandı";
                db.SaveChanges();
            }

            return RedirectToAction("FirmaTeklifler", "Teklifler");
        }

        public ActionResult SaticiTeklifler()
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "SaticiTeklifler";
            projeEntities1 db = new projeEntities1();
            DateTime localTime = DateTime.Now;

            var urunler = (from u in db.urunler where u.bitis_tarihi <= localTime && u.teklif_durumu == "Evet" && u.uyeler.yetki == "Üye" select u).ToList();
            if (urunler.Count() > 0)
            {
                foreach (var item in urunler)
                {
                    item.teklif_durumu = "Hayır";
                    db.SaveChanges();
                }
            }

            var teklifler = from t in db.teklifler where t.urunler.bitis_tarihi <= localTime && t.urunler.teklif_durumu == "Hayır" && t.urunler.uyeler.yetki == "Üye" select t;

            return View(teklifler.ToList().OrderBy(x=>x.urunler.bitis_tarihi));
        }

        public ActionResult Satici_teklif_islem(int id)
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "SaticiTeklifler";
            projeEntities1 db = new projeEntities1();
            DateTime localTime = DateTime.Now;

            var teklifler = from t in db.teklifler where t.id == id && t.urunler.bitis_tarihi <= localTime && t.urunler.teklif_durumu == "Hayır" && t.urunler.uyeler.yetki == "Üye" select t;

            return View(teklifler.First());
        }

        [HttpPost]
        public ActionResult Satici_teklif_islem(FormCollection col,int teklif_id)
        {
            projeEntities1 db = new projeEntities1();
            var teklif = (from t in db.teklifler where t.id == teklif_id && t.urunler.uyeler.yetki == "Üye" select t).First();
            int teklif_islem = Convert.ToInt32(col["teklif_islem"]);

            if(teklif_islem == 1)
            {
                // Üyelerin bakiyelerini geri yükle
                var group = (from gr in db.teklif_miktarlari
                             where gr.urun_id == teklif.urun_id
                             group gr by new { gr.uye_id }
                             into g
                             select new { g.Key.uye_id, Toplam = g.Sum(a => a.teklif_butonlari.miktar) }).ToList();

                var uyeler = (from u in db.uyeler select u).ToList();

                foreach (var uye in group)
                {
                    foreach (var item in uyeler)
                    {
                        if (uye.uye_id == item.id)
                        {
                            item.bakiye += uye.Toplam;
                            db.SaveChanges();
                            break;
                        }
                    }
                }

                // Teklif_miktarları tablosunu temizle
                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(c => c.urun_id == teklif.urun_id));

                // Urunu resetle
                var urun = (from u in db.urunler where u.id == teklif.urun_id select u).First();
                urun.baslangic_tarihi = null;
                urun.bitis_tarihi = null;
                urun.yayin_durumu = "Hayır";

                // Urun teklif butonlarını sil
                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(c => c.urun_id == teklif.urun_id));

                // Teklifler tablosunu temizle
                db.teklifler.RemoveRange(db.teklifler.Where(c => c.id == teklif.id));
                db.SaveChanges();
            }
            else if(teklif_islem == 2)
            {
                // Kaybeden Üyelerin bakiyelerini geri yükle ve mesaj gönder
                var group = (from gr in db.teklif_miktarlari
                             where gr.urun_id == teklif.urun_id && gr.uye_id != teklif.uye_id
                             group gr by new { gr.uye_id }
                             into g
                             select new { g.Key.uye_id, Toplam = g.Sum(a => a.teklif_butonlari.miktar) }).ToList();

                var uyeler = (from u in db.uyeler where u.id != teklif.uye_id select u).ToList();

                foreach (var uye in group)
                {
                    foreach (var item in uyeler)
                    {
                        if (uye.uye_id == item.id)
                        {
                            item.bakiye += uye.Toplam;

                            var mesaj = new mesajlasmalar();
                            mesaj.uye_id = uye.uye_id;
                            mesaj.tarih = DateTime.Now;
                            mesaj.mesaj = "<p> '" + teklif.urunler.adi + "' isimli ürünün açık arttırmasını kazanamadınız. Yaptığınız teklifler bakiyenize geri yüklenmiştir.</p>";
                            mesaj.durum = "Yeni";
                            mesaj.tur = "Alınan";
                            mesaj.mesaj_turu = "Açıkarttırma";

                            db.mesajlasmalar.Add(mesaj);
                            db.SaveChanges();
                            break;
                        }
                    }
                }

                // Kazanan kişi adına kargo oluştur ve mesaj gönder

                var kargo = new kargolar();
                kargo.tur = "Satıcı-Üye";
                kargo.durum = "Kargolama İşlemi Bekleniyor";
                kargo.tarih = DateTime.Now;
                kargo.urun_id = teklif.urun_id;
                kargo.uye_id = teklif.uye_id;
                db.kargolar.Add(kargo);

                var mesaj2 = new mesajlasmalar();
                mesaj2.uye_id = teklif.uye_id;
                mesaj2.tarih = DateTime.Now;
                mesaj2.mesaj = "<p> '" + teklif.urunler.adi + "' isimli ürünün açık arttırmasını kazandınız. 'Alınan Kargolarım' sayfasından ürünü takip edebilirsiniz.</p>";
                mesaj2.durum = "Yeni";
                mesaj2.tur = "Alınan";
                mesaj2.mesaj_turu = "Açıkarttırma";

                db.mesajlasmalar.Add(mesaj2);
                db.SaveChanges();

                // Teklif Miktarları tablosunu temizle
                db.teklif_miktarlari.RemoveRange(db.teklif_miktarlari.Where(c => c.urun_id == teklif.urun_id));

                // Urunu pasif yap
                var urun = (from u in db.urunler where u.id == teklif.urun_id select u).First();
                urun.durum = "Pasif";

                // Urun teklif butonlarını sil
                db.urun_teklif_butonlari.RemoveRange(db.urun_teklif_butonlari.Where(c => c.urun_id == teklif.urun_id));

                // Teklifler tablosu durumunu Onaylandı yap
                teklif.durum = "Onaylandı";
                db.SaveChanges();


                var uye_ = (from u in db.uyeler where u.id == teklif.urunler.uye_id select u).First();

                var mesaj3 = new mesajlasmalar();
                mesaj3.uye_id = uye_.id;
                mesaj3.tarih = DateTime.Now;
                mesaj3.mesaj = "<p> '" + teklif.urunler.adi + "' isimli ürününüzün açık arttırmasından '" + teklif.teklif + "' TL teklif ile ürünü '" + teklif.uyeler.adsoy + "' üyesi kazanmıştır. Ürün için bir kargo talebi oluşturulmuştur. Gönderilen kargolarım sayfasından bu talebi takip edebilir ve ürünün kargo bilgisini güncelleye bilirsiniz. Ürün kazanan üyeye ulaştığı doğrulandığında gerekli komisyonlar düşülerek bakiyenize teklif miktarı eklenecektir.</p>";
                mesaj3.durum = "Yeni";
                mesaj3.tur = "Alınan";
                mesaj3.mesaj_turu = "Kargo Talebi";

                db.mesajlasmalar.Add(mesaj3);
                db.SaveChanges();
            }

            return RedirectToAction("SaticiTeklifler", "Teklifler");
        }

        public ActionResult TeklifButonlari()
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "TeklifButonlari";

            if (TempData["ButonEkleSonuc"] != null)
            {
                ViewBag.ButonEkleSonuc = TempData["ButonEkleSonuc"].ToString();
            }
            if (TempData["ButonEkleHata"] != null)
            {
                ViewBag.ButonEkleHata = TempData["ButonEkleHata"].ToString();
            }
            if (TempData["ButonSil"] != null)
            {
                ViewBag.ButonSil = TempData["ButonSil"].ToString();
            }
            if (TempData["ButonSilHata"] != null)
            {
                ViewBag.ButonSilHata = TempData["ButonSilHata"].ToString();
            }

            projeEntities1 db = new projeEntities1();

            return View(db.teklif_butonlari.ToList());
        }

        [HttpPost]
        public ActionResult TeklifButonuEkle(FormCollection col)
        {
            projeEntities1 db = new projeEntities1();



            if (Convert.ToInt32(col["miktar"]) > 0)
            {
                int mik = Convert.ToInt32(col["miktar"]);
                var tbvar = (from t in db.teklif_butonlari where t.miktar == mik select t).ToList();
                if (tbvar.Count() == 0)
                {
                    var tb = new teklif_butonlari();
                    tb.miktar = Convert.ToInt32(col["miktar"]);
                    db.teklif_butonlari.Add(tb);
                    db.SaveChanges();

                    TempData["ButonEkleSonuc"] = Convert.ToInt32(col["miktar"]) + "TL lik buton eklendi.";
                }
                else
                {
                    TempData["ButonEkleHata"] = "Aynı miktarlar eklenemez.";
                }

            }
            else
            {
                TempData["ButonEkleHata"] = "0'dan büyük değerler eklenebilir.";
            }



            return RedirectToAction("TeklifButonlari", "Teklifler");
        }

        public ActionResult TeklifButonuSil(int id)
        {
            projeEntities1 db = new projeEntities1();
            var tek_var = (from t in db.urun_teklif_butonlari where t.teklif_butonu_id == id select t).ToList();
            if (tek_var.Count() == 0)
            {
                db.teklif_butonlari.RemoveRange(db.teklif_butonlari.Where(x => x.id == id));
                db.SaveChanges();

                TempData["ButonSil"] = "Buton silindi";
            }
            else
            {
                TempData["ButonSilHata"] = "Silmeye çalıştığınız buton, başka ürünlerde kullanılıyor. Silmek için önce bu ürünleri silmelisiniz.";
            }


            return RedirectToAction("TeklifButonlari", "Teklifler");
        }

        public ActionResult TeklifZamanlari()
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "TeklifZamanlari";

            if (TempData["ZamanEkleSonuc"] != null)
            {
                ViewBag.ZamanEkleSonuc = TempData["ZamanEkleSonuc"].ToString();
            }
            if (TempData["ZamanEkleHata"] != null)
            {
                ViewBag.ZamanEkleHata = TempData["ZamanEkleHata"].ToString();
            }
            if (TempData["ZamanSil"] != null)
            {
                ViewBag.ZamanSil = TempData["ZamanSil"].ToString();
            }

            projeEntities1 db = new projeEntities1();
            return View(db.zamanlar.ToList().OrderByDescending(x => x.zaman_dilimi).ThenBy(x => Convert.ToInt32(x.zaman)));
        }

        public ActionResult TeklifZamaniEkle()
        {
            ViewBag.ActiveMenu = "Teklifler";
            ViewBag.ActiveSubMenu = "TeklifZamanlari";

            return View();
        }

        [HttpPost]
        public ActionResult TeklifZamaniEkle(FormCollection col)
        {
            var tur = col["tur"];
            var zaman_dilimi = col["zaman_dilimi"];
            var zaman = col["zaman"];

            projeEntities1 db = new projeEntities1();

            var z_var = (from z in db.zamanlar where z.tur == tur && z.zaman_dilimi == zaman_dilimi && z.zaman == zaman select z).ToList();
            if (z_var.Count() == 0)
            {
                var zamanlar = new zamanlar();
                zamanlar.tur = tur;
                zamanlar.zaman_dilimi = zaman_dilimi;
                zamanlar.zaman = zaman;
                db.zamanlar.Add(zamanlar);
                db.SaveChanges();

                TempData["ZamanEkleSonuc"] = col["zaman"] + " " + col["zaman_dilimi"] + " " + col["tur"] + " zamanı eklendi.";
            }
            else
            {
                TempData["ZamanEkleHata"] = col["zaman"] + " " + col["zaman_dilimi"] + " " + col["tur"] + " zamanı zaten bulunmakta.";
            }



            return RedirectToAction("TeklifZamanlari", "Teklifler");
        }

        public ActionResult TeklifZamanSil(int id)
        {
            projeEntities1 db = new projeEntities1();

            db.zamanlar.RemoveRange(db.zamanlar.Where(x => x.id == id));
            db.SaveChanges();

            TempData["ZamanSil"] = "Zaman silindi.";

            return RedirectToAction("TeklifZamanlari", "Teklifler");
        }


    }
}