using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDashboardPreventCrossSiteRequestForgery.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CustomDashboardController : DashboardController
    {
        public CustomDashboardController(DashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null): base(configurator, dataProtectionProvider) { 
        }

    }


    //public sealed class DashboardValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    //{
    //    public void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        if (filterContext == null)
    //        {
    //            throw new ArgumentNullException(nameof(filterContext));
    //        }

    //        HttpContextBase httpContext = filterContext.HttpContext;
    //        HttpRequestBase request = httpContext.Request;
    //        HttpCookie cookie = request.Cookies[AntiForgeryConfig.CookieName];
    //        string token = request.Headers["__RequestVerificationToken"];
    //        if (string.IsNullOrEmpty(token))
    //        {
    //            token = request.Form["__RequestVerificationToken"];
    //        }
    //        AntiForgery.Validate(cookie?.Value, token);
    //    }
    //}
}
