using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace AspNetCoreDashboardPreventCrossSiteRequestForgery {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {
            
            DashboardExportSettings.CompatibilityMode = DashboardExportCompatibilityMode.Restricted;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddResponseCompression()
                .AddMvc()
                .ConfigureApplicationPartManager((manager) => {
                    var dashboardApplicationParts = manager.ApplicationParts.Where(part => part is AssemblyPart && ((AssemblyPart)part).Assembly == typeof(DashboardController).Assembly).ToList();
                    foreach (var partToRemove in dashboardApplicationParts) {
                        manager.ApplicationParts.Remove(partToRemove);
                    }
                });

            services
                .AddDevExpressControls()
                .AddSingleton<CustomDashboardConfigurator>();

            services.AddAntiforgery(options => {
                // Set Cookie properties using CookieBuilder properties†.
                options.FormFieldName = "X-CSRF-TOKEN";
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseDevExpressControls();

            app.UseRouting();
            app.UseEndpoints(endpoints => {
                EndpointRouteBuilderExtension.MapDashboardRoute(endpoints, "dashboardControl", "CustomDashboard");
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    public class CustomDashboardConfigurator : DashboardConfigurator {
        public CustomDashboardConfigurator(IConfiguration configuration, IWebHostEnvironment hostingEnvironment) {

            this.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

            DashboardFileStorage dashboardFileStorage = new DashboardFileStorage(hostingEnvironment.ContentRootFileProvider.GetFileInfo("Data/Dashboards").PhysicalPath);
            this.SetDashboardStorage(dashboardFileStorage);

            DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

            // Registers an SQL data source.
            DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
            sqlDataSource.DataProcessingMode = DataProcessingMode.Client;
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("Categories")
                .Join("Products", "CategoryID")
                .SelectAllColumns()
                .Build("Products_Categories");
            sqlDataSource.Queries.Add(query);
            dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());

            // Registers an Object data source.
            DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
            dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

            // Registers an Excel data source.
            DashboardExcelDataSource excelDataSource = new DashboardExcelDataSource("Excel Data Source");
            excelDataSource.FileName = hostingEnvironment.ContentRootFileProvider.GetFileInfo("Data/Sales.xlsx").PhysicalPath;
            excelDataSource.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));
            dataSourceStorage.RegisterDataSource("excelDataSource", excelDataSource.SaveToXml());

            this.SetDataSourceStorage(dataSourceStorage);

            this.DataLoading += (s, e) => {
                if(e.DataSourceName == "Object Data Source") {
                    e.Data = Invoices.CreateData();
                }
            };
        }
    }
}
