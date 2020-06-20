using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proje.Areas.admin
{
    public class LoginAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["email"] == null)
            {
                httpContext.Response.Redirect("/admin/Login/Index");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}