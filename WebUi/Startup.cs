using DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.API;
using Services.Implementation;
using WebUi.Hubs;


namespace WebUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<Data>(opts =>
            opts.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddSignalR();

            services.AddScoped<IChatMsgService, ChatMsgService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddSingleton<IUserService, MockUserService>();
            //services.AddSingleton<IChatMsgService, MockChatMsgService>();
            services.AddSingleton<IGameService, GameService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<BackgammonHub>("/backgammonHub");
                endpoints.MapHub<UserHub>("/userHub");
            });

        }
    }
}
