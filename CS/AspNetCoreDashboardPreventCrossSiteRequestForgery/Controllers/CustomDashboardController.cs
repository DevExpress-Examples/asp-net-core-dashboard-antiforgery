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
        public CustomDashboardController(CustomDashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null): base(configurator, dataProtectionProvider) { 
        }

    }    
}
