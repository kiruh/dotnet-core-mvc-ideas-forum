using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiltersSample.Filters;
using Ideas.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Models;

namespace ideas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // services.AddMvc(options =>
            // {
            //     options.Filters.Add(new CustomAuthorizationFilterFactory());
            // });

            services.AddDbContext<IdeasContext>(
                options => options.UseMySQL(
                    Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            // services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // services.AddSession(options =>
            // {
            //     // Set a short timeout for easy testing.
            //     options.IdleTimeout = TimeSpan.FromHours(2);
            //     //options.Cookie.HttpOnly = true;
            // });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Idea/Error");
            }

            // app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Idea}/{action=Index}/{id?}");
            });
        }
    }
}
