<!-- default file list -->
*Files to look at*:

* [Startup.cs](./CS/AspNetCoreDashboardPreventCrossSiteRequestForgery/Startup.cs)
<!-- default file list end -->
# ASP.NET Core Dashboard - How to Prevent Cross-Site Request Forgery (CSRF) Attacks

The following example shows how to apply antiforgery request validation to the ASP.NET Core Dashboard control.

## Example Overview

Follow the steps below to apply antiforgery request validation.

### Create custom dashboard controller
1. Add a custom dashboard controller.

```cs
namespace AspNetCoreDashboardPreventCrossSiteRequestForgery.Controllers {
    public class CustomDashboardController : DashboardController     {
        public CustomDashboardController(DashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null): base(configurator, dataProtectionProvider) { 
        }
    }
}
```


2. Change default routing.

```cs
EndpointRouteBuilderExtension.MapDashboardRoute(endpoints, "dashboardControl", "CustomDashboard");
```

3. Specify the controller name.

```razor
@(Html.DevExpress().Dashboard("dashboardControl1")
     ...
    .ControllerName("CustomDashboard")
)
```


###  Add validation for AntiforgeryToken
1. Add the Antiforgery service.

```cs
 services.AddAntiforgery(options => {
	// Use CookieBuilder to set Cookie properties.
	options.FormFieldName = "X-CSRF-TOKEN";
	options.HeaderName = "X-CSRF-TOKEN";
	options.SuppressXFrameOptionsHeader = false;
});
```

2. Add the AutoValidateAntiforgeryToken attribute to the custom controller.

```cs
[AutoValidateAntiforgeryToken]
public class CustomDashboardController : DashboardController {
	// ...
}
```

3. Configure the Web Dashboard control's backend options.

```razor
@inject Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf
 
@(Html.DevExpress().Dashboard("dashboardControl1")
   ...
    .BackendOptions(backendOptions => {
        backendOptions.RequestHttpHeaders(headers => {
            headers.Add("X-CSRF-TOKEN", Xsrf.GetAndStoreTokens(HttpContext).RequestToken);
        });
    })
)
```

## Documentation

- [Web Dashboard - Security Considerations](https://docs.devexpress.com/Dashboard/118651/web-dashboard/general-information/security-considerations)
- [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery)
- [CA3147: Mark verb handlers with ValidateAntiForgeryToken](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca3147)
- [ASP.NET MVC Security Best Practices - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.Mvc#4-preventing-cross-site-request-forgery-csrf)
