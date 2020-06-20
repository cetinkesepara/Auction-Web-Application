using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proje
{
    public class GirisyapAuthorize:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["uye_email"] == null)
            {
                httpContext.Response.Redirect("/Girisyap/Index");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}