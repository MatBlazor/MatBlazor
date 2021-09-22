using MatBlazor.Demo.Models;
using MatBlazor.Demo.Services;
using MatBlazor.Doc.Demo;
using MatBlazor.Demo.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace MatBlazor.Demo.ServerApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<HttpClient>();
            
            services.AddMatBlazor();
            
            services.AddRazorPages();
            services.AddServerSideBlazor(c =>
            {
                c.DetailedErrors = true;
            });
            services.AddSignalR(c =>
            {
                c.EnableDetailedErrors = true;
                c.StreamBufferCapacity = Int32.MaxValue;
                c.MaximumReceiveMessageSize = long.MaxValue;
                
            });

            var useDocFrameModel = Environment.GetEnvironmentVariable("USE_DOC_APPFRAME_DEMO") == "true";
            AppModel appModel = useDocFrameModel ?
                new DocFrameAppModel() :
                new MatBlazorDocumentationAppModel();

            services.AddDocApp(appModel);

            services.AddScoped<DemoUserService>();           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapBlazorHub(c =>
                {
                    c.ApplicationMaxBufferSize = long.MaxValue;
                    c.TransportMaxBufferSize = long.MaxValue;
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}