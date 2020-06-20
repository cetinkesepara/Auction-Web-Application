using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using proje.Areas.admin.Models;

namespace proje.Areas.admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: admin/Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection col)
        {
            string email = col["email"];
            string sifre = col["sifre"];

            projeEntities1 db = new projeEntities1();

            var admin = from adm in db.uyeler where (adm.yetki == "Yönetici" || adm.yetki == "Admin") && adm.email == email && adm.sifre == sifre && adm.durum == "Aktif" select adm;
            if(admin.Count() == 1)
            {
                var oturum = admin.First();
                Session["adsoy"] = oturum.adsoy;
                Session["yetki"] = oturum.yetki;
                Session["id"] = oturum.id;
                Session["resim"] = oturum.resim;
                Session["email"] = oturum.email;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.LoginError = "Email adresiniz veya şifreniz yanlış eğer şifrenizi unuttuysanız yönetici ile iletişime geçiniz.";
            return View(col);
        }

        public ActionResult Logout()
        {
            Session.Remove("adsoy");
            Session.Remove("yetki");
            Session.Remove("id");
            Session.Remove("resim");
            Session.Remove("email");
            return RedirectToAction("Index", "Login");
        }

    }
}