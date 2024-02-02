<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/381088552/23.2.2%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1010110)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# ASP.NET Core Dashboard - How to Prevent Cross-Site Request Forgery (CSRF) Attacks

The following example applies antiforgery request validation to the DevExpress ASP.NET Core Dashboard control.

## Example Overview

Follow the steps below to apply antiforgery request validation.

### Configure a custom dashboard controller

1. Create a custom dashboard controller. If you already have a custom controller, you can skip this step.

```cs
namespace AspNetCoreDashboardPreventCrossSiteRequestForgery.Controllers {
    public class CustomDashboardController : DashboardController {
        public CustomDashboardController(CustomDashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null): base(configurator, dataProtectionProvider) { 
        }
    }    
}
```

2. Change default routing to use the created controller.

```cs
app.UseEndpoints(endpoints => {
	endpoints.MapDashboardRoute("dashboardControl", "CustomDashboard");
	// ...
});
```

3. Specify the controller name in the Web Dashboard settings.

```razor
@(Html.DevExpress().Dashboard("dashboardControl1")
     ...
    .ControllerName("CustomDashboard")
)
```


###  Add validation for AntiforgeryToken
1. Add the `Antiforgery` service.

```cs
services.AddAntiforgery(options => {
	// Set Cookie properties using CookieBuilder properties†.
	options.FormFieldName = "X-CSRF-TOKEN";
	options.HeaderName = "X-CSRF-TOKEN";
	options.SuppressXFrameOptionsHeader = false;
});
```

2. Add the `AutoValidateAntiforgeryToken` attribute to the custom controller.

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
    .ControllerName("CustomDashboard")
    .BackendOptions(backendOptions => {
        backendOptions.RequestHttpHeaders(headers => {
            headers.Add("X-CSRF-TOKEN", Xsrf.GetAndStoreTokens(HttpContext).RequestToken);
        });
    })
)
```

## Files to Review

* [CustomDashboardController.cs](./CS/AspNetCoreDashboardPreventCrossSiteRequestForgery/Controllers/CustomDashboardController.cs)
* [Index.cshtml](./CS/AspNetCoreDashboardPreventCrossSiteRequestForgery/Pages/Index.cshtml)
* [Startup.cs](./CS/AspNetCoreDashboardPreventCrossSiteRequestForgery/Startup.cs)

## Documentation

- [Web Dashboard - Security Considerations](https://docs.devexpress.com/Dashboard/118651/web-dashboard/general-information/security-considerations)
- [Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery)
- [CA3147: Mark verb handlers with ValidateAntiForgeryToken](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca3147)

## More Examples

- [ASP.NET MVC Dashboard - How to Prevent Cross-Site Request Forgery (CSRF) Attacks](https://github.com/DevExpress-Examples/asp-net-mvc-dashboard-antiforgery)
